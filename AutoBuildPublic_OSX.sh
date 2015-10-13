#!/bin/sh

#Discover some stuff about where we are.
START_PATH=$(pwd)
THIS_FOLDER=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )

#Configuration Settings
SOLUTION_NAME=MedicalMac.sln

#Do Build
SOLUTION_PATH=$THIS_FOLDER/$SOLUTION_NAME

/Applications/Xamarin\ Studio.app/Contents/MacOS/mdtool build -c:PublicRelease -t:Clean "$SOLUTION_PATH"
/Applications/Xamarin\ Studio.app/Contents/MacOS/mdtool build -c:PublicRelease "$SOLUTION_PATH"

#Build Installer
sh $THIS_FOLDER/AnomalousMedicalMac/Installer/Public/PublicDMG.sh

#Finish up
cd "$START_PATH"