# WallForge v0.1.0 — approved brand asset integration

The project owner approved the flat navy/orange shield, firewall-brick and flame WallForge logo on 2026-11-27.

## Approved source assets
The approved logo package has been uploaded to the project Google Drive folder: https://drive.google.com/file/d/1kaqqxgb_VlTOnnrNe3GP0fzs0efJvVzO/view (`SharedSpace/WallForge/wallforge-brand-v0.1.0.zip`). It contains:
- `wallforge-horizontal.png` — login/header wordmark
- `wallforge-symbol.png` — standalone shield
- `favicon.ico`
- `icon-16.png`, `icon-32.png`, `icon-48.png`, `icon-64.png`, `icon-128.png`, `icon-192.png`, `icon-256.png`, `icon-512.png`
- `README.md`

Do not redraw or replace the approved logo. Obtain the exact ZIP attachment from the project owner/local Codex workspace, and check its contents before integration.

## Implementation checklist
1. Add original brand files to an appropriate versioned frontend assets directory.
2. Replace temporary logo placeholders in the login page and header with the approved wordmark; use the symbol at small sizes.
3. Set the document favicon and web-app icons; ensure all paths work when served by the published ASP.NET Core app.
4. Verify aspect ratio, transparent-background appearance, legibility at 16/32 px, dark and light themes, mobile layout, and accessibility alt text.
5. Update README, release notes and project docs with the approved logo.
6. Run Angular production build, .NET build, security and integration tests, and browser smoke tests.
7. Build a versioned Linux x64 installable artifact with SHA256 checksums. Keep preview/pre-release designation until release gates pass.
8. Validate on a disposable Ubuntu 24.04 VM, including install, service restart, reboot, journal, PAM, and active UFW. Do not touch existing WSL/production hosts.
9. Request independent security review for PAM helper, sudoers and systemd configuration.
10. Only then propose final v0.1.0 stable tag and GitHub Release; never label unverified preview artifacts as stable.

## Release constraints
The previously passing Docker and CI checks do not replace VM-based acceptance. No host firewall mutations are permitted as part of v0.1.0.

## Observed RC1 status (2026-11-27)
Items1–3,5–7 completed. Item4: desktop/mobile aspect ratio, existing UI contrast/white backing and alt text verified; served ICO/PNG hashes match approved assets. Separate theme-switch and exhaustive16px visual review not claimed. Item8 BLOCKED (no approved VM); item9 independent review pending; item10 stable release not approved/published. Exact package source and evidence in TEST_REPORT_v0.1.0.md.
