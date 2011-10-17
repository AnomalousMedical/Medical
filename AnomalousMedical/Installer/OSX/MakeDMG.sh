#!/bin/sh

# MakeDMG.sh
# 
#
# Created by Andrew Piper on 1/18/11.
# Copyright 2011 Anomalous Software. All rights reserved.

#Discover some stuff about where we are.
START_PATH=$(pwd)

APP_NAME=$1
APP_PATH=$2
DMG_NAME=$3
LAYOUT_FOLDER=$4
LICENSE_FOLDER=$5

sudo rm -r "$DMG_NAME"
sudo rm "$DMG_NAME.dmg"

mkdir -p "$DMG_NAME"

cp -r "$APP_PATH/$APP_NAME.app" "$DMG_NAME/$APP_NAME.app"

dropdmg --layout-folder="$LAYOUT_FOLDER" --license-folder="$LICENSE_FOLDER" --internet-enabled "$DMG_NAME"

sudo rm -r "$DMG_NAME"