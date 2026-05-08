[CmdletBinding()]
param(
  [string] $CertPath = "signature.pfx",
  [string] $CertPassword = 'your_certificate_password',
  [string] $Version = "1.1.0"
)

if ([string]::IsNullOrWhiteSpace($CertPassword))
{
  $CertPassword = $env:IPERF_CERT_PASSWORD
  if ([string]::IsNullOrWhiteSpace($CertPassword))
  {
    Write-Error "Aucun mot de passe fourni. Définissez IPERF_CERT_PASSWORD ou utilisez -CertPassword (déconseillé)."
    exit 1
  }
}

if ([string]::IsNullOrWhiteSpace($Version))
{
  $csprojPath = Join-Path $PSScriptRoot "IperfApp.csproj"
  if (-not (Test-Path $csprojPath))
  {
    Write-Error "IperfApp.csproj introuvable à '$csprojPath'. Spécifiez -Version manuellement."
    exit 1
  }
  [xml]$csproj = Get-Content $csprojPath
  $Version = $csproj.Project.PropertyGroup.Version | Where-Object { $_ } | Select-Object -First 1
  if ([string]::IsNullOrWhiteSpace($Version))
  {
    Write-Error "Impossible de lire la version depuis IperfApp.csproj."
    exit 1
  }
  Write-Host "Version lue depuis IperfApp.csproj : $Version" -ForegroundColor DarkCyan
}

$publishFolder = "bin\Release\net10.0-windows\win-x64\publish"
$exePath = "$publishFolder\IperfApp.exe"
$setupPath = "Output\Speedtest_Iperf_v${Version}_Setup.exe"
$timestamp = "http://timestamp.digicert.com"

$iscc = "C:\Users\piete\AppData\Local\Programs\Inno Setup 6\ISCC.exe"

if (-not (Test-Path $iscc))
{
  Write-Error "Inno Setup introuvable. Installez-le ou ajoutez ISCC.exe au PATH."
  exit 1
}

$ErrorActionPreference = "Stop"

Write-Host "--- 1. NETTOYAGE ET COMPILATION .NET ---" -ForegroundColor Cyan
if (Test-Path "bin") { Remove-Item -Recurse -Force "bin" }
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true

if (-not (Test-Path $exePath))
{
  Write-Error "IperfApp.exe introuvable après la publication."
  exit 1
}

$securePass = ConvertTo-SecureString $CertPassword -AsPlainText -Force
$cert = [System.Security.Cryptography.X509Certificates.X509Certificate2]::new(
  (Resolve-Path $CertPath).Path,
  $securePass
)

Write-Host "--- 2. SIGNATURE DE L'APPLICATION ---" -ForegroundColor Cyan
Set-AuthenticodeSignature -FilePath $exePath -Certificate $cert -TimestampServer $timestamp
Start-Sleep -Seconds 2

Write-Host "--- 3. CRÉATION DE L'INSTALLATEUR (ISCC) ---" -ForegroundColor Cyan
& $iscc "installer.iss"

if (-not (Test-Path $setupPath))
{
  Write-Error "L'installateur '$setupPath' n'a pas été généré."
  exit 1
}

Write-Host "--- 4. SIGNATURE DE L'INSTALLATEUR ---" -ForegroundColor Cyan
Set-AuthenticodeSignature -FilePath $setupPath -Certificate $cert -TimestampServer $timestamp

Write-Host "--- SUCCÈS : Application et installateur signés ! ---" -ForegroundColor Green
