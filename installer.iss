#define MyAppName "Speedtest Iperf"
#define MyAppVersion "1.0.2"
#define MyAppPublisher "Lucas PIETERS"
#define MyAppCopyright "Copyright © 2026 Lucas PIETERS"
#define MyAppExeName "IperfApp.exe"

[Setup]
AppId={{A1B2C3D4-E5F6-4G7H-8I9J-K1L2M3N4O5P6}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}

; --- GESTION DES PRIVILÈGES (CHOIX UTILISATEUR) ---
; 'lowest' permet de ne pas demander d'admin par défaut
PrivilegesRequired=lowest
; 'dialog' affiche une page au début demandant : "Installer pour moi" ou "Pour tous les utilisateurs"
PrivilegesRequiredOverridesAllowed=dialog

; {autopf} basculera automatiquement entre 'Program Files' (admin) et 'Local AppData' (standard)
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
SetupIconFile=Resources\favicon.ico

; --- CONFIGURATION DU FICHIER DE SORTIE ---
OutputBaseFilename=Speedtest_Iperf_v{#MyAppVersion}_Setup
OutputDir=Output
Compression=lzma
SolidCompression=yes
WizardStyle=modern

; --- MÉTA-DONNÉES DU SETUP.EXE ---
VersionInfoDescription=Outil de mesure de débit réseau basé sur l'outil Iperf3
VersionInfoProductName={#MyAppName}
VersionInfoProductVersion={#MyAppVersion}
VersionInfoCompany={#MyAppPublisher}
VersionInfoCopyright={#MyAppCopyright}
VersionInfoVersion={#MyAppVersion}.0

[Languages]
Name: "french"; MessagesFile: "compiler:Languages\French.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
; Utilisation de 'Flags: ignoreversion' pour éviter les conflits
Source: "bin\Release\net10.0-windows\win-x64\publish\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion restartreplace
Source: "bin\Release\net10.0-windows\win-x64\publish\Resources\*"; DestDir: "{app}\Resources"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[UninstallDelete]
; Supprime tous les rapports CSV créés par l'appli dans le dossier d'installation
Type: files; Name: "{app}\*.csv"
; Supprime tout le dossier 'Resources' au cas où des logs y auraient été créés
Type: filesandordirs; Name: "{app}\Resources"
; Force la suppression du dossier principal, même s'il reste des résidus
Type: filesandordirs; Name: "{app}"

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent