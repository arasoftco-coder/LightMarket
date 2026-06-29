# ۱. تعریف مسیرها (آدرس‌ها را مطابق سیستم خودتان تغییر دهید)
$UiProjectPath = "C:\projects\lightmarket"
$DotNetWwwrootPath = "C:\Projects\LightMarket\src\WebAPI\LampEcommerce.WebAPI\wwwroot" # مسیر پوشه wwwroot پروژه دات‌نت

Write-Host "--- Starting UI Build Process ---" -ForegroundColor Cyan

# ۲. ورود به پوشه پروژه فرانت‌اند و اجرای Build
cd $UiProjectPath
npm run build

# بررسی اینکه آیا بیلد با موفقیت انجام شده یا خیر
if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: Build failed. Process aborted." -ForegroundColor Red
    Read-Host "Press Enter to exit..."
    exit
}

Write-Host "--- Cleaning old files from wwwroot ---" -ForegroundColor Yellow

# ۳. پاک کردن محتویات قبلی پوشه wwwroot (در صورت وجود پوشه)
if (Test-Path $DotNetWwwrootPath) {
    Remove-Item -Recurse -Force "$DotNetWwwrootPath\*"
} else {
    # اگر پوشه wwwroot از قبل وجود نداشته باشد، آن را می‌سازد
    New-Item -ItemType Directory -Path $DotNetWwwrootPath | Out-Null
}

Write-Host "--- Copying new build files to wwwroot ---" -ForegroundColor Green

# ۴. کپی کردن فایل‌های جدید از پوشه dist به wwwroot
# نکته: نام پوشه خروجی شما (dist/lightmarket-web) را بر اساس پروژه خودتان دقیق کنید
$BuildOutputDir = "$UiProjectPath\dist\light-market\browser" 

if (Test-Path $BuildOutputDir) {
    Copy-Item -Path "$BuildOutputDir\*" -Destination $DotNetWwwrootPath -Recurse -Force
    Write-Host "Success: UI Published and copied to wwwroot successfully!" -ForegroundColor Green
} else {
    Write-Host "Error: Build output directory not found at $BuildOutputDir" -ForegroundColor Red
}

Read-Host "Press Enter to close..."