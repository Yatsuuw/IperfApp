#define MyAppName "Speedtest Iperf"
#define MyAppVersion "1.1.0"
#define MyAppPublisher "Lucas PIETERS"
#define MyAppCopyright "Copyright © 2026 Lucas PIETERS"
#define MyAppExeName "IperfApp.exe"

[Setup]
; --- IDENTIFIANTS ET MÉTADONNÉES ---
; GUID généré avec [System.Guid]::NewGuid() le 07/05/2026
AppId={{4F9A3C1B-82D7-4E5A-B6F0-C3D2E1A09847}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppCopyright={#MyAppCopyright}
VersionInfoVersion={#MyAppVersion}
VersionInfoCompany={#MyAppPublisher}
VersionInfoDescription=Installateur pour {#MyAppName}
VersionInfoCopyright={#MyAppCopyright}
VersionInfoProductName={#MyAppName}

; --- 1. BI-INSTALLATION (Choix Local ou Administrateur) ---
PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=dialog
ArchitecturesInstallIn64BitMode=x64compatible

DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
SetupIconFile=Resources\favicon.ico

OutputBaseFilename=Speedtest_Iperf_v{#MyAppVersion}_Setup
OutputDir=Output
Compression=lzma
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "french"; MessagesFile: "compiler:Languages\French.isl"

[Tasks]
; --- 2. CASE ICÔNE SUR LE BUREAU PRÉCOCHÉE ---
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"

[Dirs]
; --- FIX CRITIQUE : Donne les droits de modification aux utilisateurs sur le dossier d'installation ---
Name: "{app}"; Permissions: users-modify

[Files]
; --- 3. INSTALLATION PAR DESSUS ---
Source: "bin\Release\net10.0-windows\win-x64\publish\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion restartreplace
Source: "bin\Release\net10.0-windows\win-x64\publish\Resources\*"; DestDir: "{app}\Resources"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[UninstallDelete]
; --- 4. DÉSINSTALLATION TOTALE (config inclus) ---
Type: filesandordirs; Name: "{app}"

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent
