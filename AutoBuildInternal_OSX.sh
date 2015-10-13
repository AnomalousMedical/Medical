#!/bin/sh

#Discover some stuff about where we are.
START_PATH=$(pwd)
THIS_FOLDER=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )

#Configuration Settings
SOLUTION_NAME=MedicalMac.sln

#Do Build
SOLUTION_PATH=$THIS_FOLDER/$SOLUTION_NAME

#cleaning doesn't work for some reason
xbuild "$SOLUTION_PATH" /p:Configuration=Release /t:clean
xbuild "$SOLUTION_PATH" /p:Configuration=Release

#Build Installer
sh $THIS_FOLDER/AnomalousMedicalMac/Installer/Internal/InternalDMG.sh

#Finish up
cd "$START_PATH"