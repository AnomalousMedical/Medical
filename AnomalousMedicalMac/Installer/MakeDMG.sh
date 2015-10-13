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

rm -r "$DMG_NAME"
rm "$DMG_NAME.dmg"

mkdir -p "$DMG_NAME"

# Use pax to copy, keeps sym links valid, have to be in the folder with the app bundle for this to work however.
cd $APP_PATH
pax -rw "$APP_NAME.app" "$START_PATH/$DMG_NAME"
cd $START_PATH

dropdmg --layout-folder="$LAYOUT_FOLDER" --license-folder="$LICENSE_FOLDER" --internet-enabled "$DMG_NAME"

rm -r "$DMG_NAME"