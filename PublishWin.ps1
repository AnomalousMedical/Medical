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
$publishDir = "$scriptPath\Publish"
try{
    Push-Location $scriptPath

    try{
        Push-Location $appFolder -ErrorAction Stop
        dotnet restore $csproj; Test-Error "Could not dotnet restore $csproj."
        dotnet publish -r win-x64 -c Release; Test-Error "Error publishing app to $publishDir"

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