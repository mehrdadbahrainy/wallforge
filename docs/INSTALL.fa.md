# نصب آزمایشی WallForge نسخه 0.1.0 روی اوبونتو

> این نسخه هنوز برای انتشار عمومی و استفاده روی سرور عملیاتی تایید نشده است. آزمایش‌های واقعی PAM و UFW را روی ماشین مجازی اوبونتو انجام دهید.

## پیش‌نیازها

اوبونتو، SDK نسخه 10 دات‌نت، Node.js نسخه 24، ابزارهای UFW و sudo و کتابخانه PAM. برای ساخت Helper بومی PAM به gcc و libpam0g-dev نیاز داریم.

## ساخت نرم‌افزار

```bash
git clone https://github.com/mehrdadbahrainy/wallforge.git
cd wallforge
git checkout feature/v0.1.0-foundation

dotnet publish src/WallForge.Api/WallForge.Api.csproj -c Release -o ./out/api
cd frontend
npm install
npm run build
cd ..
mkdir -p out/api/wwwroot
cp -a frontend/dist/wallforge/browser/. out/api/wwwroot/
```

## احراز هویت با حساب‌های اوبونتو

```bash
sudo groupadd --system --force wallforge-admin
sudo groupadd --system --force wallforge-viewer
sudo usermod -aG wallforge-admin YOUR_UBUNTU_USERNAME
sudo install -o root -g root -m 0644 ops/pam.d/wallforge /etc/pam.d/wallforge
```

ورود با PAM انجام می‌شود و نرم‌افزار پایگاه داده رمز عبور جداگانه ندارد. فقط کاربران محلی عضو یکی از دو گروه بالا اجازه ورود دارند. ورود root ممنوع است. در نسخه 0.1.0 حتی مدیر هم نمی‌تواند تنظیمات فایروال یا کاربران را تغییر دهد.

## مجوز محدود خواندن وضعیت UFW

```bash
sudo useradd --system --home-dir /var/lib/wallforge --shell /usr/sbin/nologin wallforge-api
sudo apt-get install -y gcc libpam0g-dev
gcc -O2 -Wall -Wextra -Werror -fstack-protector-strong -D_FORTIFY_SOURCE=3 \
  -o /tmp/wallforge-pam-auth src/WallForge.PamHelper/wallforge-pam-auth.c -lpam
sudo mkdir -p /usr/local/libexec
sudo install -o root -g root -m 0755 /tmp/wallforge-pam-auth /usr/local/libexec/wallforge-pam-auth
sudo install -o root -g root -m 0440 ops/sudoers.d/wallforge /etc/sudoers.d/wallforge
sudo visudo -cf /etc/sudoers.d/wallforge
```

مجوز sudo فقط دو فرمان مشخص را پوشش می‌دهد: Helper بومی احراز هویت PAM بدون آرگومان و فرمان ثابت و فقط‌خواندنی `ufw status numbered`. رمز عبور از طریق ورودی استاندارد به Helper ارسال می‌شود، نه آرگومان فرمان. از تغییر مجوزهای `/etc/shadow` یا دادن دسترسی نامحدود sudo به سرویس خودداری کنید.

## اجرای آزمایشی با HTTPS

```bash
ASPNETCORE_URLS=https://localhost:5443 dotnet out/api/WallForge.Api.dll
```

گواهی TLS باید روی سیستم مورد اعتماد باشد. ورود از طریق HTTP عادی کار نخواهد کرد، چون کوکی‌ها فقط در اتصال امن ارسال می‌شوند.

برای اجرای سرویس systemd، فایل `ops/systemd/wallforge-api.service` در مخزن وجود دارد. بعد از نصب در `/opt/wallforge/api`، یک reverse proxy دارای TLS در شبکه مدیریت راه‌اندازی کنید. برنامه نباید مستقیما در اینترنت عمومی باز باشد.

## تست‌های ضروری پیش از انتشار

- موفق و ناموفق بودن ورود از طریق PAM با حساب واقعی اوبونتو
- منع ورود root، کاربران قفل‌شده و حساب‌های خارج از گروه‌های مجاز
- تفکیک مدیر و بازدیدکننده و بررسی دسترسی Audit
- لغو فوری مجوز بعد از خروج کاربر از گروه‌های WallForge
- مشاهده دقیق UFW روی ماشین مجازی و اطمینان از نبود مسیر تغییر فایروال
- اجرای موفق Build، تست‌ها و ثبت نتیجه در فایل مدیریت پروژه

**محدودیت‌ها:** مدیریت قوانین و کاربران هنوز فقط‌خواندنی است. قابلیت Safe Apply در نسخه 0.2.0 اضافه خواهد شد. گزارش رخدادهای پنل در حافظه نگهداری می‌شود و بعد از راه‌اندازی مجدد پاک می‌شود. گزارش سیستمی در journal ثبت می‌شود. تغییر قفل حساب پس از ورود ممکن است تا پایان نشست کوتاه فعلی اثر نکند. این موضوع باید پیش از انتشار روی اوبونتو آزمایش شود.
