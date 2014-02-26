; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "Anomalous Medical"
#define MyAppVersion GetFileVersion("S:\Medical\PublicRelease\AnomalousMedical.exe")
#define MyAppPublisher "Anomalous Medical"
#define MyAppURL "http://www.anomalousmedical.com"
#define MyAppExeName "AnomalousMedical.exe"

#if Exec('S:\DRM\CodeKey\SignPublicRelease.bat') != 0
#error Could not sign
#endif

#if Exec('S:\Medical\AnomalousMedical\Installer\Windows\CopyPublicPluginDlls.bat') != 0
#error Could not copy plugin dlls
#endif

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{E2774DD7-4B71-40EB-AD2F-A7B5E32A9605}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\Anomalous Medical\Platform
DefaultGroupName=Anomalous Medical
LicenseFile=S:\Medical\AnomalousMedical\Installer\License\en.rtf
OutputDir=S:\Medical\PublicRelease\Setups
OutputBaseFilename=AnomalousMedicalSetup
Compression=lzma
SolidCompression=yes
SignTool=AnomalousMedicalSign $qAnomalous Medical Installer$q $f

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: S:\Medical\PublicRelease\AnomalousMedical.exe; DestDir: {app}; Flags: ignoreversion 
Source: S:\Medical\PublicRelease\BulletPlugin.dll; DestDir: {app}; Flags: ignoreversion
Source: S:\Medical\PublicRelease\BulletWrapper.dll; DestDir: {app}; Flags: ignoreversion
Source: S:\Medical\PublicRelease\cg.dll; DestDir: {app}; Flags: ignoreversion
Source: S:\Medical\PublicRelease\Engine.dll; DestDir: {app}; Flags: ignoreversion
Source: S:\Medical\PublicRelease\MyGUIPlugin.dll; DestDir: {app}; Flags: ignoreversion
Source: S:\Medical\PublicRelease\MyGUIWrapper.dll; DestDir: {app}; Flags: ignoreversion
Source: S:\Medical\PublicRelease\libRocketPlugin.dll; DestDir: {app}; Flags: ignoreversion
Source: S:\Medical\PublicRelease\libRocketWrapper.dll; DestDir: {app}; Flags: ignoreversion
Source: S:\Medical\PublicRelease\OgreCWrapper.dll; DestDir: {app}; Flags: ignoreversion
Source: S:\Medical\PublicRelease\OgreMain.dll; DestDir: {app}; Flags: ignoreversion
Source: S:\Medical\PublicRelease\OgreOverlay.dll; DestDir: {app}; Flags: ignoreversion
Source: S:\Medical\PublicRelease\OgrePlugin.dll; DestDir: {app}; Flags: ignoreversion
Source: S:\Medical\PublicRelease\OSHelper.dll; DestDir: {app}; Flags: ignoreversion
Source: S:\Medical\PublicRelease\Plugin_CgProgramManager.dll; DestDir: {app}; Flags: ignoreversion
Source: S:\Medical\PublicRelease\RenderSystem_Direct3D9.dll; DestDir: {app}; Flags: ignoreversion
Source: S:\Medical\PublicRelease\ShapeLoader.dll; DestDir: {app}; Flags: ignoreversion
Source: S:\Medical\PublicRelease\Simulation.dll; DestDir: {app}; Flags: ignoreversion
Source: S:\Medical\PublicRelease\SoundPlugin.dll; DestDir: {app}; Flags: ignoreversion
Source: S:\Medical\PublicRelease\SoundWrapper.dll; DestDir: {app}; Flags: ignoreversion
Source: S:\Medical\PublicRelease\Standalone.dll; DestDir: {app}; Flags: ignoreversion
Source: S:\Medical\PublicRelease\WinMTDriver.dll; DestDir: {app}; Flags: ignoreversion
Source: S:\Medical\PublicRelease\Zip.dll; DestDir: {app}; Flags: ignoreversion
Source: S:\Medical\PublicRelease\DotNetZip.dll; DestDir: {app}; Flags: ignoreversion
Source: S:\Medical\PublicRelease\Mono.Anomalous.Security.dll; DestDir: {app}; Flags: ignoreversion
Source: S:\Medical\PublicRelease\AnomalousMedical.dat; DestDir: {app}; Flags: ignoreversion
Source: S:\Medical\PublicRelease\IntroductionTutorial.dat; DestDir: {app}; Flags: ignoreversion

;Open AL
Source: "S:\dependencies\InstallerDependencies\Windows\oalinst.exe"; DestDir: "{tmp}"; Flags: ignoreversion deleteafterinstall
;VS 2010 Redistributable
Source: "S:\dependencies\InstallerDependencies\Windows\vcredist_x86.exe"; DestDir: "{tmp}"; Flags: ignoreversion deleteafterinstall
;DX Required Files
Source: "S:\dependencies\InstallerDependencies\Windows\DirectX9c\DXSETUP.exe"; DestDir: "{tmp}\DirectX9c"; Flags: ignoreversion deleteafterinstall
Source: "S:\dependencies\InstallerDependencies\Windows\DirectX9c\dsetup32.dll"; DestDir: "{tmp}\DirectX9c"; Flags: ignoreversion deleteafterinstall
Source: "S:\dependencies\InstallerDependencies\Windows\DirectX9c\DSETUP.dll"; DestDir: "{tmp}\DirectX9c"; Flags: ignoreversion deleteafterinstall
Source: "S:\dependencies\InstallerDependencies\Windows\DirectX9c\dxdllreg_x86.cab"; DestDir: "{tmp}\DirectX9c"; Flags: ignoreversion deleteafterinstall
Source: "S:\dependencies\InstallerDependencies\Windows\DirectX9c\dxupdate.cab"; DestDir: "{tmp}\DirectX9c"; Flags: ignoreversion deleteafterinstall
;DX June 2010 Files
Source: "S:\dependencies\InstallerDependencies\Windows\DirectX9c\Jun2010_d3dx9_43_x86.cab"; DestDir: "{tmp}\DirectX9c"; Flags: ignoreversion deleteafterinstall
Source: "S:\dependencies\InstallerDependencies\Windows\DirectX9c\Jun2010_D3DCompiler_43_x86.cab"; DestDir: "{tmp}\DirectX9c"; Flags: ignoreversion deleteafterinstall
;.Net 4.0
Source: S:\dependencies\InstallerDependencies\Windows\dotNetFx40_Client_setup.exe; DestDir: {tmp}; 

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{tmp}\oalinst.exe"; Parameters: "/s"; StatusMsg: "Installing OpenAL";
Filename: "{tmp}\vcredist_x86.exe"; Parameters: "/q /norestart"; StatusMsg: "Installing Visual Studio 2012 Redistributable (x86)";
Filename: "{tmp}\DirectX9c\DXSETUP.exe"; Parameters: "/silent"; StatusMsg: "Installing DirectX 9.0";
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, "&", "&&")}}"; Flags: nowait postinstall skipifsilent

[Code]
procedure checkdotnetfx4();
var
  resultCode: Integer;
	version: cardinal;
begin
	RegQueryDWordValue(HKLM, 'Software\Microsoft\NET Framework Setup\NDP\v4\Client', 'Install', version);
	if version < 1 then 
	begin
    if MsgBox('You need to install the Microsoft .Net Framework 4.0.'#13#10'If you are connected to the internet you can do this now.'#13#10'Would you like to continue?', mbConfirmation, MB_YESNO) = IDYES then
      begin
        Exec(ExpandConstant('{tmp}\dotNetFx40_Client_setup.exe'), '/norestart', '', SW_SHOW, ewWaitUntilTerminated, resultCode);
      end
    else
      begin
        MsgBox('You must install the Microsoft .Net Framework 4.0 for this program to work.'#13#10'Please visit www.anomalousmedical.com for more info.', mbInformation, MB_OK)
      end;
   end;
//	   if resultCode=3010 then
	   //Restart required
end;

procedure CurStepChanged(CurStep: TSetupStep);
begin
  if(CurStep = ssPostInstall) then
    checkdotnetfx4();
end;
