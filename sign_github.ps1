# Configuration
$pfxPath = "signature.pfx"
$password = 'Mot de passe'
$publishFolder = "bin\Release\net10.0-windows\win-x64\publish"
$exePath = "$publishFolder\IperfApp.exe"
$setupPath = "Output\Speedtest_Iperf_v1.0.4_Setup.exe"
$timestamp = "http://timestamp.digicert.com"

# Si erreur, script s'arrête
$ErrorActionPreference = "Stop"

Write-Host "--- 1. NETTOYAGE ET COMPILATION .NET ---" -ForegroundColor Cyan
if (Test-Path "bin") { Remove-Item -Recurse -Force "bin" }

# Lancement de la publication
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true

# Vérification que le fichier a bien été créé
if (-not (Test-Path $exePath)) { throw "Erreur : Le fichier IperfApp.exe n'a pas été généré." }

# Chargement du certificat
$cert = [System.Security.Cryptography.X509Certificates.X509Certificate2]::new($pfxPath, $password)

Write-Host "--- 2. SIGNATURE DE L'APPLICATION SOURCE ---" -ForegroundColor Cyan
Set-AuthenticodeSignature -FilePath $exePath -Certificate $cert -TimestampServer $timestamp

# Pause pour Windows
Start-Sleep -Seconds 2

Write-Host "--- 3. CRÉATION DE L'INSTALLATEUR (ISCC) ---" -ForegroundColor Cyan
$iscc = "C:\Users\piete\AppData\Local\Programs\Inno Setup 6\ISCC.exe"
& $iscc "installer.iss"

Write-Host "--- 4. SIGNATURE DE L'INSTALLATEUR FINAL ---" -ForegroundColor Cyan
Set-AuthenticodeSignature -FilePath $setupPath -Certificate $cert -TimestampServer $timestamp

Write-Host "--- SUCCÈS : L'APPLICATION ET LE SETUP SONT SIGNÉS ! ---" -ForegroundColor Green