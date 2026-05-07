<#
.SYNOPSIS
    Compile, signe et package IperfApp en un seul script.

.DESCRIPTION
    1. Nettoie et compile l'application via `dotnet publish`.
    2. Signe IperfApp.exe avec le certificat PFX spécifié.
    3. Génère l'installateur via Inno Setup (ISCC.exe).
    4. Signe l'installateur produit.

.PARAMETER CertPath
    Chemin vers le fichier .pfx de signature (défaut : signature.pfx).

.PARAMETER CertPassword
    Mot de passe du certificat PFX.
    RECOMMANDÉ : ne pas passer cette valeur en clair dans la ligne de commande.
    Préférez la variable d'environnement IPERF_CERT_PASSWORD (voir .EXAMPLE).
    Si absent et si IPERF_CERT_PASSWORD n'est pas définie, le script s'arrête.

.PARAMETER Version
    Version de l'application (ex : "1.1.0"). Utilisée pour nommer le setup.
    Si absent, la version est lue automatiquement depuis IperfApp.csproj.

.EXAMPLE
    # Méthode recommandée : définir le mot de passe dans une variable d'environnement
    # (ne laisse pas de trace dans l'historique PowerShell)
    $env:IPERF_CERT_PASSWORD = Read-Host -Prompt "Mot de passe du certificat" -AsSecureString | `
        ConvertFrom-SecureString -AsPlainText
    .\sign_github.ps1

.EXAMPLE
    # En CI/CD : définir la variable d'environnement au niveau du système
    # ou via les secrets du pipeline (GitHub Actions, Azure DevOps, etc.)
    # Puis lancer sans argument :
    .\sign_github.ps1

.EXAMPLE
    # Méthode non recommandée (mot de passe visible dans l'historique) :
    # .\sign_github.ps1 -CertPassword "<votre-mot-de-passe>"
    # Utilisez uniquement dans un environnement totalement isolé et de confiance.
#>
[CmdletBinding()]
param(
  [string] $CertPath     = "signature.pfx",
  [string] $CertPassword = "",
  [string] $Version      = ""
)

# Si le mot de passe n'est pas passé en paramètre, on lit la variable d'environnement
if ([string]::IsNullOrWhiteSpace($CertPassword))
{
  $CertPassword = $env:IPERF_CERT_PASSWORD
  if ([string]::IsNullOrWhiteSpace($CertPassword))
  {
    Write-Error "Aucun mot de passe fourni. Définissez IPERF_CERT_PASSWORD ou utilisez -CertPassword (déconseillé)."
    exit 1
  }
}

# Si la version n'est pas fournie, on la lit depuis IperfApp.csproj
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

# --- Chemins ---
$publishFolder = "bin\Release\net10.0-windows\win-x64\publish"
$exePath       = "$publishFolder\IperfApp.exe"
$setupPath     = "Output\Speedtest_Iperf_v${Version}_Setup.exe"
$timestamp     = "http://timestamp.digicert.com"

# Détection automatique d'Inno Setup via le registre
$isccKey  = "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Inno Setup 6_is1"
$isccDir  = (Get-ItemProperty -Path $isccKey -ErrorAction SilentlyContinue)?.InstallLocation
$iscc     = if ($isccDir) { Join-Path $isccDir "ISCC.exe" } else { "ISCC.exe" }

if (-not (Test-Path $iscc))
{
  Write-Error "Inno Setup introuvable. Installez-le ou ajoutez ISCC.exe au PATH."
  exit 1
}

# Arrêt du script sur toute erreur
$ErrorActionPreference = "Stop"

# --- Étape 1 : Nettoyage et compilation ---
Write-Host "--- 1. NETTOYAGE ET COMPILATION .NET ---" -ForegroundColor Cyan
if (Test-Path "bin") { Remove-Item -Recurse -Force "bin" }
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true

if (-not (Test-Path $exePath))
{
  Write-Error "IperfApp.exe introuvable après la publication."
  exit 1
}

# --- Étape 2 : Chargement du certificat ---
$securePass = ConvertTo-SecureString $CertPassword -AsPlainText -Force
$cert = [System.Security.Cryptography.X509Certificates.X509Certificate2]::new(
  (Resolve-Path $CertPath).Path,
  $securePass
)

# --- Étape 3 : Signature de l'exécutable ---
Write-Host "--- 2. SIGNATURE DE L'APPLICATION ---" -ForegroundColor Cyan
Set-AuthenticodeSignature -FilePath $exePath -Certificate $cert -TimestampServer $timestamp
Start-Sleep -Seconds 2

# --- Étape 4 : Génération de l'installateur ---
Write-Host "--- 3. CRÉATION DE L'INSTALLATEUR (ISCC) ---" -ForegroundColor Cyan
& $iscc "installer.iss"

if (-not (Test-Path $setupPath))
{
  Write-Error "L'installateur '$setupPath' n'a pas été généré."
  exit 1
}

# --- Étape 5 : Signature de l'installateur ---
Write-Host "--- 4. SIGNATURE DE L'INSTALLATEUR ---" -ForegroundColor Cyan
Set-AuthenticodeSignature -FilePath $setupPath -Certificate $cert -TimestampServer $timestamp

Write-Host "--- SUCCÈS : Application et installateur signés ! ---" -ForegroundColor Green
