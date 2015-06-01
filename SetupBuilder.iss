;
; LicenseManager - Inno Setup script file
; =======================================
;
; Description:  Generate a setup file for Windows
; Author:       Jan Bucher
;
; HowTo "Build a setup file":
;   1. Build LicenseManager with Visual Studio 2013 (or higher) in RELEASE Mode
;   2. Install InnoSetup (if not already done)
;   3. Open this file in the Inno Setup Compiler
;   4. Change the constant "SourceDir" to the directory of the LicenseManager sources
;   5. Change the constant "MyAppVersion" to the new version
;   6. Build!
;

#define SourceDir "C:\git\LicenseManager"
; SourceDir Tipp: Do not use a network directory! Do not add a "\" at the end!

#define MyAppName "LicenseManager"
#define MyAppVersion "0.5"
#define MyAppPublisher "DEV-JAN"
#define MyAppURL "http://dev-jan.github.io/LicenseManager/"
#define MyAppExeName "LicenseManager.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
AppId={{85AC1314-643F-4AA4-A9DD-598E35CBE125}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
LicenseFile={#SourceDir}\LICENSE
OutputBaseFilename={#MyAppName}_{#MyAppVersion}_Setup
Compression=lzma
SolidCompression=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "{#SourceDir}\LicenseManager.UI\bin\Release\LicenseManager.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#SourceDir}\LicenseManager.UI\bin\Release\LicenseManager.DataAccess.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#SourceDir}\LicenseManager.UI\bin\Release\LicenseManager.Logic.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#SourceDir}\LicenseManager.UI\bin\Release\LicenseManager.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#SourceDir}\LICENSE"; DestDir: "{app}"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

