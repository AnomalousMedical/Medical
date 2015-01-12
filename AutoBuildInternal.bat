::Configuration Settings
set SolutionName=Medical.sln

::Less likely to need to change these.
set ThisFolder=%~dp0
set RootDependencyFolder=%ThisFolder%..\
set BuildCommand="C:\Program Files (x86)\MSBuild\12.0\Bin\msbuild.exe" /m
set CurrentDirectory=%CD%
set InnosetupCommand="C:\Program Files (x86)\Inno Setup 5\Compil32.exe" /cc
set SignCommand=call "%RootBuildFolder%DRM\CodeKey\AnomalousMedicalSign.bat"
set OutputFolder=%ThisFolder%Release\
set SolutionPath=%ThisFolder%%SolutionName%

%BuildCommand% "%SolutionPath%" /property:Configuration=Debug;Platform="Any CPU" /target:Clean
%BuildCommand% "%SolutionPath%" /property:Configuration=Debug;Platform="Any CPU"

%BuildCommand% "%SolutionPath%" /property:Configuration=Release;Platform="Any CPU" /target:Clean
%BuildCommand% "%SolutionPath%" /property:Configuration=Release;Platform="Any CPU"

%BuildCommand% "%SolutionPath%" /property:Configuration=Debug;Platform="x64" /target:Clean
%BuildCommand% "%SolutionPath%" /property:Configuration=Debug;Platform="x64"

%BuildCommand% "%SolutionPath%" /property:Configuration=Release;Platform="x64" /target:Clean
%BuildCommand% "%SolutionPath%" /property:Configuration=Release;Platform="x64"

%SignCommand% "Premium Features" "%OutputFolder%Premium.dll"
%SignCommand% "Dental Simulation" "%OutputFolder%DentalSim.dll"
%SignCommand% "Developer" "%OutputFolder%Developer.dll"
%SignCommand% "Editor Tools" "%OutputFolder%Editor.dll"
%SignCommand% "Smart Lecture Tools" "%OutputFolder%Lecture.dll"
%SignCommand% "Store Manager" "%OutputFolder%StoreManager.dll"
%SignCommand% "Kinect Plugin" "%OutputFolder%KinectPlugin.dll"
%SignCommand% "Movement Simulation" "%OutputFolder%Movement.dll"
%SignCommand% "Anomalous Medical" "%OutputFolder%\AnomalousMedical.exe"

echo f | xcopy /Y "%OutputFolder%Premium.dll" "%OutputFolder%Setups\Premium.dll"
echo f | xcopy /Y "%OutputFolder%DentalSim.dll" "%OutputFolder%Setups\DentalSim.dll"
echo f | xcopy /Y "%OutputFolder%Developer.dll" "%OutputFolder%Setups\Developer.dll"
echo f | xcopy /Y "%OutputFolder%Editor.dll" "%OutputFolder%Setups\Editor.dll"
echo f | xcopy /Y "%OutputFolder%Lecture.dll" "%OutputFolder%Setups\Lecture.dll"
echo f | xcopy /Y "%OutputFolder%StoreManager.dll" "%OutputFolder%Setups\StoreManager.dll"
echo f | xcopy /Y "%OutputFolder%Movement.dll" "%OutputFolder%Setups\Movement.dll"

%InnosetupCommand% %ThisFolder%AnomalousMedical\Installer\Windows\AnomalousMedicalInternal.iss