; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "SimpleVolumeControl"
#define MyAppVersion "0.0.1.2"
#define MyAppPublisher "E. Mesilin"
#define MyAppURL "https://github.com/Mesilin/SimpleVolumeControl"
#define MyAppExeName "VolumeControl.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application. Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{685F0186-BFA3-4FED-85EB-FF7A3592A432}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
DisableDirPage=yes
DisableProgramGroupPage=yes
LicenseFile=C:\Git\SimpleVolumeControl\VolumeControl\bin\Debug\net6.0-windows\lic.txt
InfoBeforeFile=C:\Git\SimpleVolumeControl\VolumeControl\bin\Debug\net6.0-windows\about.txt
; Uncomment the following line to run in non administrative install mode (install for current user only.)
;PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=dialog
OutputDir=C:\Git\SimpleVolumeControl\VolumeControl\
OutputBaseFilename=Setup
SetupIconFile=C:\Git\SimpleVolumeControl\VolumeControl\ico.ico
Compression=lzma
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "C:\Git\SimpleVolumeControl\VolumeControl\bin\Debug\net6.0-windows\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Git\SimpleVolumeControl\VolumeControl\bin\Debug\net6.0-windows\Gma.System.MouseKeyHook.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Git\SimpleVolumeControl\VolumeControl\bin\Debug\net6.0-windows\Microsoft.Win32.TaskScheduler.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Git\SimpleVolumeControl\VolumeControl\bin\Debug\net6.0-windows\VolumeControl.deps.json"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Git\SimpleVolumeControl\VolumeControl\bin\Debug\net6.0-windows\VolumeControl.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Git\SimpleVolumeControl\VolumeControl\bin\Debug\net6.0-windows\VolumeControl.dll.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Git\SimpleVolumeControl\VolumeControl\bin\Debug\net6.0-windows\VolumeControl.runtimeconfig.json"; DestDir: "{app}"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall shellexec skipifsilent

