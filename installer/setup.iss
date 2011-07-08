#define MyAppName "Torshify"
#define MyAppVersion "0.5"
#define MyAppPublisher "Torstein Auensen"
#define MyAppURL "http://www.torsh.net/blog"
#define MyAppExeName "Torshify.Client.exe"

#include "dotnet4.iss"

[Setup]
AppId={{9189D461-E642-4C47-9690-2AF1469688F5}
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
OutputBaseFilename=setup
Compression=lzma
SolidCompression=yes
PrivilegesRequired=poweruser

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Files]
Source: "..\src\Torshify.Client\bin\Release\Torshify.Client.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\src\Torshify.Client\bin\Release\bass.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\src\Torshify.Client\bin\Release\Bass.Net.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\src\Torshify.Client\bin\Release\libspotify.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\src\Torshify.Client\bin\Release\log4net.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\src\Torshify.Client\bin\Release\Microsoft.Practices.Prism.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\src\Torshify.Client\bin\Release\Microsoft.Practices.Prism.UnityExtensions.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\src\Torshify.Client\bin\Release\Microsoft.Practices.ServiceLocation.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\src\Torshify.Client\bin\Release\Microsoft.Practices.Unity.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\src\Torshify.Client\bin\Release\System.Windows.Interactivity.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\src\Torshify.Client\bin\Release\Torshify.Client.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\src\Torshify.Client\bin\Release\Torshify.Client.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\src\Torshify.Client\bin\Release\Torshify.Client.Infrastructure.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\src\Torshify.Client\bin\Release\Torshify.Client.Modules.Core.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\src\Torshify.Client\bin\Release\Torshify.Client.Spotify.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\src\Torshify.Client\bin\Release\Torshify.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\src\Torshify.Client\bin\Release\WpfShaderEffects.dll"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, "&", "&&")}}"; Flags: nowait postinstall skipifsilent runascurrentuser

[Code]
function InitializeSetup(): Boolean;
begin
    if not IsDotNetDetected('v4\Full', 0) then begin
        MsgBox('Torshify requires Microsoft .NET Framework 4.0.'#13#13
            'Please use install this version,'#13
            'and then re-run the Torshify setup program.', mbInformation, MB_OK);
        result := false;
    end else
        result := true;
end;
