[Setup]
AppName=Rancher Inventory
AppVersion=1.0
DefaultDirName={pf}\RancherInventory
DefaultGroupName=Rancher Inventory
UninstallDisplayIcon={app}\Rancher.exe
OutputDir=C:\Users\mayan\OneDrive\Desktop\Installer
OutputBaseFilename=RancherInventorySetup
Compression=lzma
SolidCompression=yes
DisableProgramGroupPage=yes
WizardStyle=modern

[Files]
; Replace this path with your actual published folder (from dotnet publish)
Source: "C:\Users\mayan\.net\Rancher\Publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\Rancher Inventory"; Filename: "{app}\Rancher.exe"
Name: "{group}\Uninstall Rancher Inventory"; Filename: "{uninstallexe}"

[Run]
Filename: "{app}\Rancher.exe"; Description: "Launch Rancher Inventory"; Flags: nowait postinstall skipifsilent
