#define MyAppName "Speedtest Iperf"
#define MyAppVersion "1.0.3"
#define MyAppPublisher "Lucas PIETERS"
#define MyAppCopyright "Copyright © 2026 Lucas PIETERS"
#define MyAppExeName "IperfApp.exe"

[Setup]
; AppId unique pour identifier le programme dans le registre
AppId={{A1B2C3D4-E5F6-4G7H-8I9J-K1L2M3N4O5P6}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}

PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=dialog

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
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "bin\Release\net10.0-windows\win-x64\publish\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion restartreplace
Source: "bin\Release\net10.0-windows\win-x64\publish\Resources\*"; DestDir: "{app}\Resources"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[UninstallDelete]
Type: files; Name: "{app}\*.csv"
Type: filesandordirs; Name: "{app}\Resources"
Type: filesandordirs; Name: "{app}"

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[Code]
/////////////////////////////////////////////////////////////////////
// DÉTECTION ET DÉSINSTALLATION PRÉALABLE                          //
/////////////////////////////////////////////////////////////////////

function GetUninstallString(): String;
var
  sUninstPath: String;
  sUninstString: String;
begin
  // Chemin de la clé de désinstallation Inno Setup
  sUninstPath := ExpandConstant('Software\Microsoft\Windows\CurrentVersion\Uninstall\{#emit SetupSetting("AppId")}_is1');
  sUninstString := '';
  
  // Test dans HKLM (Machine) puis HKCU (Utilisateur actuel)
  if not RegQueryStringValue(HKLM, sUninstPath, 'UninstallString', sUninstString) then
    RegQueryStringValue(HKCU, sUninstPath, 'UninstallString', sUninstString);
    
  Result := sUninstString;
end;

function InitializeSetup(): Boolean;
var
  V: Integer;
  sUninstString: String;
begin
  Result := True; // Par défaut, on continue l'installation
  sUninstString := GetUninstallString();

  if sUninstString <> '' then
  begin
    // Message personnalisé demandé
    if MsgBox('Une version de {#MyAppName} est déjà installée sur votre ordinateur.' #13#10 #13#10 'Souhaitez-vous désinstaller l''application ?', mbConfirmation, MB_YESNO) = IDYES then
    begin
      sUninstString := RemoveQuotes(sUninstString);
      // Exécution silencieuse du désinstalleur existant
      if Exec(sUninstString, '/SILENT /NORESTART /SUPPRESSMSGBOXES', '', SW_SHOW, ewWaitUntilTerminated, V) then
      begin
        Sleep(1000); // Temps de latence pour la libération des fichiers par Windows
      end;
    end;
    // Note : On ne retourne pas 'False' ici car l'installeur actuel doit 
    // continuer son exécution pour installer la nouvelle version.
  end;
end;