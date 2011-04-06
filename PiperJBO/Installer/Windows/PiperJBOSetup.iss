; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "Piper's JBO"
#define MyAppVersion "2.0"
#define MyAppPublisher "Anomalous Medical"
#define MyAppURL "http://www.anomalousmedical.com"
#define MyAppExeName "PiperJBO.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{B6E818F6-FE6A-43E8-BA0E-66361CDEA8BC}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
LicenseFile=S:\Medical\PiperJBO\PiperJBO_EULA.rtf
OutputDir=S:\Medical\PublicRelease\Setups
OutputBaseFilename=PiperJBOSetup
Compression=lzma
SolidCompression=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "S:\Medical\PublicRelease\PiperJBO.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "S:\Medical\PublicRelease\BulletPlugin.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "S:\Medical\PublicRelease\BulletWrapper.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "S:\Medical\PublicRelease\cg.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "S:\Medical\PublicRelease\Engine.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "S:\Medical\PublicRelease\MyGUIPlugin.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "S:\Medical\PublicRelease\MyGUIWrapper.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "S:\Medical\PublicRelease\OgreCWrapper.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "S:\Medical\PublicRelease\OgreMain.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "S:\Medical\PublicRelease\OgrePlugin.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "S:\Medical\PublicRelease\OSHelper.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "S:\Medical\PublicRelease\Plugin_CgProgramManager.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "S:\Medical\PublicRelease\RenderSystem_Direct3D9.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "S:\Medical\PublicRelease\ShapeLoader.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "S:\Medical\PublicRelease\Simulation.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "S:\Medical\PublicRelease\SoundPlugin.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "S:\Medical\PublicRelease\SoundWrapper.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "S:\Medical\PublicRelease\Standalone.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "S:\Medical\PublicRelease\WinMTDriver.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "S:\Medical\PublicRelease\Zip.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "S:\Medical\PublicRelease\PiperJBO.dat"; DestDir: "{app}"; Flags: ignoreversion
Source: "S:\Medical\PublicRelease\Doc\PiperJBOManual.htb"; DestDir: "{app}\Doc"; Flags: ignoreversion
Source: "S:\dependencies\InstallerDependencies\Windows\dotnetfx35setup.exe"; DestDir: "{tmp}"; Flags: ignoreversion deleteafterinstall
Source: "S:\dependencies\InstallerDependencies\Windows\oalinst.exe"; DestDir: "{tmp}"; Flags: ignoreversion deleteafterinstall
Source: "S:\dependencies\InstallerDependencies\Windows\vcredist_x86.exe"; DestDir: "{tmp}"; Flags: ignoreversion deleteafterinstall
; DirectX 9.0
;Source: "S:\dependencies\InstallerDependencies\Windows\DirectX9c\Aug2009_D3DCompiler_42_x64.cab"; DestDir: "{tmp}\DirectX9c"; Flags: ignoreversion deleteafterinstall
Source: "S:\dependencies\InstallerDependencies\Windows\DirectX9c\Aug2009_D3DCompiler_42_x86.cab"; DestDir: "{tmp}\DirectX9c"; Flags: ignoreversion deleteafterinstall
;Source: "S:\dependencies\InstallerDependencies\Windows\DirectX9c\Aug2009_d3dcsx_42_x64.cab"; DestDir: "{tmp}\DirectX9c"; Flags: ignoreversion deleteafterinstall
Source: "S:\dependencies\InstallerDependencies\Windows\DirectX9c\Aug2009_d3dcsx_42_x86.cab"; DestDir: "{tmp}\DirectX9c"; Flags: ignoreversion deleteafterinstall
;Source: "S:\dependencies\InstallerDependencies\Windows\DirectX9c\Aug2009_d3dx10_42_x64.cab"; DestDir: "{tmp}\DirectX9c"; Flags: ignoreversion deleteafterinstall
;Source: "S:\dependencies\InstallerDependencies\Windows\DirectX9c\Aug2009_d3dx10_42_x86.cab"; DestDir: "{tmp}\DirectX9c"; Flags: ignoreversion deleteafterinstall
;Source: "S:\dependencies\InstallerDependencies\Windows\DirectX9c\Aug2009_d3dx11_42_x64.cab"; DestDir: "{tmp}\DirectX9c"; Flags: ignoreversion deleteafterinstall
;Source: "S:\dependencies\InstallerDependencies\Windows\DirectX9c\Aug2009_d3dx11_42_x86.cab"; DestDir: "{tmp}\DirectX9c"; Flags: ignoreversion deleteafterinstall
;Source: "S:\dependencies\InstallerDependencies\Windows\DirectX9c\Aug2009_d3dx9_42_x64.cab"; DestDir: "{tmp}\DirectX9c"; Flags: ignoreversion deleteafterinstall
Source: "S:\dependencies\InstallerDependencies\Windows\DirectX9c\Aug2009_d3dx9_42_x86.cab"; DestDir: "{tmp}\DirectX9c"; Flags: ignoreversion deleteafterinstall
;Source: "S:\dependencies\InstallerDependencies\Windows\DirectX9c\Aug2009_XACT_x64.cab"; DestDir: "{tmp}\DirectX9c"; Flags: ignoreversion deleteafterinstall
Source: "S:\dependencies\InstallerDependencies\Windows\DirectX9c\Aug2009_XACT_x86.cab"; DestDir: "{tmp}\DirectX9c"; Flags: ignoreversion deleteafterinstall
;Source: "S:\dependencies\InstallerDependencies\Windows\DirectX9c\Aug2009_XAudio_x64.cab"; DestDir: "{tmp}\DirectX9c"; Flags: ignoreversion deleteafterinstall
Source: "S:\dependencies\InstallerDependencies\Windows\DirectX9c\Aug2009_XAudio_x86.cab"; DestDir: "{tmp}\DirectX9c"; Flags: ignoreversion deleteafterinstall
Source: "S:\dependencies\InstallerDependencies\Windows\DirectX9c\BDA.cab"; DestDir: "{tmp}\DirectX9c"; Flags: ignoreversion deleteafterinstall
Source: "S:\dependencies\InstallerDependencies\Windows\DirectX9c\BDANT.cab"; DestDir: "{tmp}\DirectX9c"; Flags: ignoreversion deleteafterinstall
Source: "S:\dependencies\InstallerDependencies\Windows\DirectX9c\BDAXP.cab"; DestDir: "{tmp}\DirectX9c"; Flags: ignoreversion deleteafterinstall
Source: "S:\dependencies\InstallerDependencies\Windows\DirectX9c\DirectX.cab"; DestDir: "{tmp}\DirectX9c"; Flags: ignoreversion deleteafterinstall
Source: "S:\dependencies\InstallerDependencies\Windows\DirectX9c\DSETUP.dll"; DestDir: "{tmp}\DirectX9c"; Flags: ignoreversion deleteafterinstall
Source: "S:\dependencies\InstallerDependencies\Windows\DirectX9c\dsetup32.dll"; DestDir: "{tmp}\DirectX9c"; Flags: ignoreversion deleteafterinstall
Source: "S:\dependencies\InstallerDependencies\Windows\DirectX9c\DX9Helper.dll"; DestDir: "{tmp}\DirectX9c"; Flags: ignoreversion deleteafterinstall
Source: "S:\dependencies\InstallerDependencies\Windows\DirectX9c\dxdllreg_x86.cab"; DestDir: "{tmp}\DirectX9c"; Flags: ignoreversion deleteafterinstall
Source: "S:\dependencies\InstallerDependencies\Windows\DirectX9c\dxnt.cab"; DestDir: "{tmp}\DirectX9c"; Flags: ignoreversion deleteafterinstall
Source: "S:\dependencies\InstallerDependencies\Windows\DirectX9c\DXSETUP.exe"; DestDir: "{tmp}\DirectX9c"; Flags: ignoreversion deleteafterinstall
Source: "S:\dependencies\InstallerDependencies\Windows\DirectX9c\dxupdate.cab"; DestDir: "{tmp}\DirectX9c"; Flags: ignoreversion deleteafterinstall

; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{tmp}\oalinst.exe"; Parameters: "/s"; StatusMsg: "Installing OpenAL";
Filename: "{tmp}\vcredist_x86.exe"; Parameters: "/q:a"; StatusMsg: "Installing Visual Studio 2008 Redistributable (x86)";
Filename: "{tmp}\DirectX9c\DXSETUP.exe"; Parameters: "/silent"; StatusMsg: "Installing DirectX 9.0";
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, "&", "&&")}}"; Flags: nowait postinstall skipifsilent

[Code]
procedure checkdotnetfx35sp1();
var
  resultCode: Integer;
	version: cardinal;
begin
	RegQueryDWordValue(HKLM, 'Software\Microsoft\NET Framework Setup\NDP\v3.5', 'SP', version);
	if version < 1 then 
	begin
    if MsgBox('You need to install the Microsoft .Net Framework 3.5 SP1.'#13#10'If you are connected to the internet you can do this now.'#13#10'Would you like to continue?', mbConfirmation, MB_YESNO) = IDYES then
      begin
        Exec(ExpandConstant('{tmp}\dotnetfx35setup.exe'), '/norestart', '', SW_SHOW, ewWaitUntilTerminated, resultCode);
      end
    else
      begin
        MsgBox('You must install the Microsoft .Net Framework 3.5 for this program to work.'#13#10'Please visit www.anomalousmedical.com for more info.', mbInformation, MB_OK)
      end;
   end;
//	   if resultCode=3010 then
	   //Restart required
end;

procedure CurStepChanged(CurStep: TSetupStep);
begin
  if(CurStep = ssPostInstall) then
    checkdotnetfx35sp1();
end;
