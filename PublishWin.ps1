# This script will publish the project to an executable installer for Windows.

$appFolder = "AnomalousMedical"
$csproj = "AnomalousMedical.csproj"
$installerFolder = "Installer"
$installScript = "AnomalousMedical.iss"

$innosetupExe = "C:\Program Files (x86)\Inno Setup 6\ISCC.exe"

function Test-Error([string] $msg, [int] $code = 0){
    if($LastExitCode -ne $code){
        throw $msg;
    }
}

$scriptPath = Split-Path $script:MyInvocation.MyCommand.Path
try{
    Push-Location $scriptPath

    try{
        Push-Location $appFolder -ErrorAction Stop
        dotnet restore $csproj; Test-Error "Could not dotnet restore $csproj."
        dotnet publish -r win-x64 -c Release -p:PublishSingleFile=false; Test-Error "Error publishing app"
        Remove-Item "bin/Release/net5.0/win-x64/" -recurse -Include *.pdb -ErrorAction Stop

        try {
            Push-Location $installerFolder -ErrorAction Stop
            &$innosetupExe $installScript
        }
        finally {
            Pop-Location
        }
    }
    finally{
        Pop-Location
    }
}
finally{
    Pop-Location
}