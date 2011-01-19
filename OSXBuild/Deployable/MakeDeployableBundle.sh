#!/bin/sh

# MakeStandaloneBundle.sh
# 
#
# Created by Andrew Piper on 1/18/11.
# Copyright 2011 Anomalous Software. All rights reserved.


APP_NAME="PiperJBO"
BUNDLE_SOURCE_DIR="BundleSource"
BUILD_DIR="Build"
DOT_NET_EXECUTABLES="../DotNetAssemblies"
APP_BUNDLE_NAME="$APP_NAME.app"
MONO_FRAMEWORK_HOME="/Users/andrewpiper/Development/Dependencies/Mono/OSXBuild"

#Discover some stuff about where we are.
START_PATH=$(pwd)
MAIN_EXE="$START_PATH/$DOT_NET_EXECUTABLES/$APP_NAME.exe"

#Make sure the temp directories are empty by removing them
sudo rm -r "$APP_BUNDLE_NAME"

#Create the app bundle
mkdir "$APP_BUNDLE_NAME"
cp -r "$BUNDLE_SOURCE_DIR/Contents" "$APP_BUNDLE_NAME"
cp "$MAIN_EXE" "$APP_BUNDLE_NAME/Contents/Resources"
chmod 755 "$APP_BUNDLE_NAME/Contents/MacOS/PiperJBO"
rm -rf `find $APP_BUNDLE_NAME -type d -name .svn`
mkdir "$APP_BUNDLE_NAME/Contents/Frameworks"

#Copy mono framework
cp -r "$MONO_FRAMEWORK_HOME/Mono.framework" "$APP_BUNDLE_NAME/Contents/Frameworks/Mono.framework"

#Finish up
cd "$START_PATH"