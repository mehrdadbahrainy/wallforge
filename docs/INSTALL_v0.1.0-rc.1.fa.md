# نصب WallForge v0.1.0-rc.1 — Pre-release

این بسته Linux x64 یک نامزد انتشار آزمایشی است، نه نسخه پایدار. فقط در کانتینر Ubuntu 24.04 آزمایشی و روی localhost استفاده شود. installer عمداً اجرای خارج از Docker را رد می‌کند؛ نصب واقعی VM، systemd و reboot هنوز تأیید نشده است. فایل unit برای بررسی بعدی همراه بسته است، نه به‌عنوان سرویس آزموده‌شده.

۱. فایل `wallforge-v0.1.0-rc.1-linux-x64.tar.gz` و `SHA256SUMS.txt` را در یک پوشه قرار دهید. در PowerShell:

```powershell
Get-FileHash -Algorithm SHA256 .\wallforge-v0.1.0-rc.1-linux-x64.tar.gz
```

خروجی را با SHA256SUMS.txt مقایسه کنید. کانتینر باید فقط `127.0.0.1:5443` را منتشر کند؛ privileged، host network و Docker socket را اضافه نکنید.

۲. در همان کانتینر Ubuntu آزمایشی، بسته را به پوشه جدیدی مانند `/tmp/wallforge-rc1` منتقل و باز کنید. نام کانتینر محیط فعلی `wallforge-local-lab-ubuntu-1` است:

```powershell
docker cp .\wallforge-v0.1.0-rc.1-linux-x64.tar.gz wallforge-local-lab-ubuntu-1:/tmp/wallforge-rc1.tar.gz
docker exec wallforge-local-lab-ubuntu-1 bash -lc 'mkdir -p /tmp/wallforge-rc1 && tar -xzf /tmp/wallforge-rc1.tar.gz -C /tmp/wallforge-rc1'
```

۳. پیش از ارتقا، اجرای قبلی WallForge را متوقف و فایل‌های API، startup و sudoers قبلی را برای بازگشت حفظ کنید. API و sudoers باید با هم ارتقا یابند. سپس:

```powershell
docker exec -it wallforge-local-lab-ubuntu-1 bash /tmp/wallforge-rc1/install.sh
docker exec -it wallforge-local-lab-ubuntu-1 bash /opt/wallforge/start.sh
```

برای نصب جدید، installer رمز آزمایشی یکتا برای `wfadmin` می‌خواهد؛ در نصب مجدد، حساب و رمز موجود را حفظ می‌کند. هیچ رمز واقعی، توکن یا کلید خصوصی را در Drive ثبت نکنید. وابستگی‌های سیستم از مخازن Ubuntu نصب می‌شوند؛ بسته self-contained است و برای اجرا به نصب SDK دات‌نت یا Node نیاز ندارد.

۴. آدرس `https://localhost:5443` را باز کنید. گواهی محلی self-signed است؛ اعتماد به آن جایگزین TLS عملیاتی نیست. ورود با حساب محلی عضو `wallforge-admin` یا `wallforge-viewer` انجام می‌شود؛ Viewer نباید Activity را ببیند. نشست ۲۰دقیقه‌ای و بدون تمدید لغزان است. انقضا با درخواست بعدی شناسایی می‌شود.

۵. UFW و حساب‌ها فقط‌خواندنی هستند. هیچ فایروال میزبان/عملیاتی را فعال، غیرفعال یا تغییر ندهید. UFW فعال IPv4/IPv6 در VM واقعی، journald پایدار و reboot از این آزمایش Docker نتیجه‌گیری نمی‌شوند.

برای بازگشت، فقط API فعلی را متوقف کنید و جفت startup/sudoers و باینری‌های متناظر قبلی را بازگردانید. در محیط فعلی، نسخه پیش از RC در `/opt/wallforge/security-api` محفوظ است؛ مسیر دقیق نسخه پشتیبان در گزارش تست ثبت می‌شود. هم‌زمان دو API را روی یک پورت اجرا نکنید.

معیارهای باز: تأیید مستقل امنیت و PR، پذیرش VM/systemd/reboot/journald و سیاست‌های واقعی PAM/UFW. هیچ merge، تگ نهایی یا GitHub Release با این بسته انجام نشده است.
