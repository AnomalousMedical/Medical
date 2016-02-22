::Configuration Settings
set SolutionName=Medical.sln

::Less likely to need to change these.
set ThisFolder=%~dp0
set RootBuildFolder=%ThisFolder%..\
set BuildCommand="msbuild.exe" /m
set CurrentDirectory=%CD%
set InnosetupCommand="C:\Program Files (x86)\Inno Setup 5\Compil32.exe" /cc
set SignCommand=call "%RootBuildFolder%DRM\CodeKey\AnomalousMedicalSign.bat"
set OutputFolder=%ThisFolder%Release\
set SolutionPath=%ThisFolder%%SolutionName%

%BuildCommand% "%SolutionPath%" /property:Configuration=Release;Platform="Any CPU" /target:Clean,Build

%SignCommand% "Store Manager" "%OutputFolder%StoreManager.dll"
%SignCommand% "Anomalous Medical" "%OutputFolder%\AnomalousMedical.exe"

echo f | xcopy /Y "%OutputFolder%StoreManager.dll" "%OutputFolder%Setups\StoreManager.dll"

%InnosetupCommand% %ThisFolder%AnomalousMedical\Installer\Windows\AnomalousMedicalInternal.iss