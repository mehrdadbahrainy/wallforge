/*
 * WallForge PAM verifier — fixed-purpose root helper for Ubuntu.
 * Input on stdin: uint16 BE username length, uint16 BE password length,
 * then exact username/password UTF-8 byte sequences. Output: ASCII '1' or '0'.
 *
 * Must be root-owned and invoked only through the argument-exact sudoers rule.
 * Never grant a shell or a wildcard sudo privilege to wallforge-api.
 */
#define _GNU_SOURCE
#include <security/pam_appl.h>
#include <sys/prctl.h>
#include <sys/resource.h>
#include <sys/types.h>
#include <pwd.h>
#include <stdlib.h>
#include <stdio.h>
#include <stdint.h>
#include <string.h>
#include <unistd.h>
#include <errno.h>
#include <signal.h>

#define USER_MAX 32
#define PASS_MAX 1024

static void clear_sensitive(void *buf, size_t length) {
    volatile unsigned char *cursor = (volatile unsigned char *)buf;
    while (length--) *cursor++ = 0;
}

static int read_exact(int fd, void *buffer, size_t length) {
    unsigned char *cursor = (unsigned char *)buffer;
    while (length) {
        ssize_t bytes = read(fd, cursor, length);
        if (bytes < 0 && errno == EINTR) continue;
        if (bytes <= 0) return -1;
        cursor += bytes;
        length -= (size_t)bytes;
    }
    return 0;
}

static int valid_user(const char *name, size_t len) {
    if (!len || len > USER_MAX || strcmp(name, "root") == 0) return 0;
    for (size_t i = 0; i < len; i++) {
        unsigned char ch = (unsigned char)name[i];
        if (i == 0 && !((ch >= 'a' && ch <= 'z') || ch == '_')) return 0;
        if (i && !((ch >= 'a' && ch <= 'z') || (ch >= '0' && ch <= '9') ||
                   ch == '_' || ch == '-' || (i == len - 1 && ch == '$'))) return 0;
    }
    return 1;
}

static int conversation(int n, const struct pam_message **messages,
                        struct pam_response **responses, void *context) {
    if (n <= 0 || n > 8 || !messages || !responses || !context) return PAM_CONV_ERR;
    struct pam_response *reply = calloc((size_t)n, sizeof(*reply));
    if (!reply) return PAM_BUF_ERR;
    const char *password = (const char *)context;
    for (int i = 0; i < n; ++i) {
        if (!messages[i]) goto fail;
        if (messages[i]->msg_style == PAM_PROMPT_ECHO_OFF) {
            reply[i].resp = strdup(password);
            if (!reply[i].resp) goto fail;
        } else if (messages[i]->msg_style != PAM_TEXT_INFO &&
                   messages[i]->msg_style != PAM_ERROR_MSG) {
            goto fail; /* challenge/response not supported: fail closed */
        }
    }
    *responses = reply;
    return PAM_SUCCESS;
fail:
    for (int i = 0; i < n; ++i) {
        if (reply[i].resp) {
            clear_sensitive(reply[i].resp, strlen(reply[i].resp));
            free(reply[i].resp);
        }
    }
    free(reply);
    *responses = NULL;
    return PAM_CONV_ERR;
}

int main(void) {
    char username[USER_MAX + 1] = {0};
    char password[PASS_MAX + 1] = {0};
    unsigned char header[4];
    char result = '0';
    pam_handle_t *handle = NULL;
    int pam_status = PAM_CONV_ERR;

    /* No core dumps, no accidental secret-bearing logs or unbounded prompts. */
    struct rlimit core_limit = {0, 0};
    (void)setrlimit(RLIMIT_CORE, &core_limit);
    (void)prctl(PR_SET_DUMPABLE, 0, 0, 0, 0);
    alarm(10);

    if (geteuid() != 0) goto end;
    if (read_exact(STDIN_FILENO, header, sizeof(header)) != 0) goto end;
    size_t user_length = (size_t)header[0] * 256U + header[1];
    size_t pass_length = (size_t)header[2] * 256U + header[3];
    if (user_length == 0 || user_length > USER_MAX || pass_length == 0 || pass_length > PASS_MAX) goto end;
    if (read_exact(STDIN_FILENO, username, user_length) != 0 ||
        read_exact(STDIN_FILENO, password, pass_length) != 0) goto end;
    username[user_length] = '\0';
    password[pass_length] = '\0';
    if (strlen(username) != user_length || strlen(password) != pass_length) goto end;
    if (!valid_user(username, user_length)) goto end;
    struct passwd *entry = getpwnam(username);
    if (!entry || entry->pw_uid < 1000 || entry->pw_uid == 65534) goto end;

    struct pam_conv conv = {conversation, password};
    pam_status = pam_start("wallforge", username, &conv, &handle);
    if (pam_status != PAM_SUCCESS || !handle) goto end;
    pam_status = pam_authenticate(handle, PAM_SILENT);
    if (pam_status != PAM_SUCCESS) goto end;
    pam_status = pam_acct_mgmt(handle, PAM_SILENT);
    if (pam_status == PAM_SUCCESS) result = '1';

end:
    if (handle) (void)pam_end(handle, pam_status);
    clear_sensitive(password, sizeof(password));
    clear_sensitive(username, sizeof(username));
    ssize_t written = write(STDOUT_FILENO, &result, 1);
    return written == 1 ? 0 : 1; /* authentication denial is not an executable error */
}
