#!/bin/sh

# MakeStandaloneBundle.sh
# 
#
# Created by Andrew Piper on 1/18/11.
# Copyright 2011 Anomalous Software. All rights reserved.

#Discover some stuff about where we are.
START_PATH=$(pwd)

APP_NAME="PiperJBO"
BUNDLE_SOURCE_DIR="BundleSource"
BUILD_DIR="Build"
APP_BUNDLE_NAME="$APP_NAME.app"
DEVELOPMENT_HOME="/Users/andrewpiper/Development"
MONO_FRAMEWORK_HOME="$DEVELOPMENT_HOME/Dependencies/Mono/OSXBuild"
ENGINE_HOME="$DEVELOPMENT_HOME/Engine"
MEDICAL_HOME="$DEVELOPMENT_HOME/Medical"
DEPENDENCIES_HOME="$DEVELOPMENT_HOME/Dependencies"

DOT_NET_EXECUTABLES="$MEDICAL_HOME/OSXBuild/DotNetAssemblies"
MAIN_EXE="$DOT_NET_EXECUTABLES/$APP_NAME.exe"

#Remove the old bundle
sudo rm -r "$APP_BUNDLE_NAME"

#Create the app bundle
mkdir "$APP_BUNDLE_NAME"
cp -r "$BUNDLE_SOURCE_DIR/Contents" "$APP_BUNDLE_NAME"
chmod 755 "$APP_BUNDLE_NAME/Contents/MacOS/$APP_NAME"
rm -rf `find $APP_BUNDLE_NAME -type d -name .svn`
mkdir "$APP_BUNDLE_NAME/Contents/Frameworks"

#Copy program files
cp "$MAIN_EXE" "$APP_BUNDLE_NAME/Contents/Resources"
cp "$DOT_NET_EXECUTABLES/"*.dll "$APP_BUNDLE_NAME/Contents/Resources"

#Copy the mac native dylibs
cp "$ENGINE_HOME/OSXBuild/Release/"*.dylib "$APP_BUNDLE_NAME/Contents/Resources"

#Copy the Medical native dylibs
cp "$MEDICAL_HOME/OSXBuild/OSHelper/build/Release/libOSHelper.dylib" "$APP_BUNDLE_NAME/Contents/Resources"

#Copy the wx.NET native dylibs
cp "$DEPENDENCIES_HOME/wxDotNet/Build/OSXBuild/wxDotNet/build/Release/libwx-c.dylib" "$APP_BUNDLE_NAME/Contents/Resources"

#Copy the ogre frameworks
mkdir "$APP_BUNDLE_NAME/Contents/Frameworks/Cg.framework"
mkdir "$APP_BUNDLE_NAME/Contents/Frameworks/Ogre.framework"
mkdir "$APP_BUNDLE_NAME/Contents/Frameworks/Ogre.framework/Versions"
mkdir "$APP_BUNDLE_NAME/Contents/Frameworks/Ogre.framework/Versions/1.7.1"

cp "$DEPENDENCIES_HOME/Ogre/OSXBuild/lib/Release/"*.dylib "$APP_BUNDLE_NAME/Contents/Resources"
cp "$DEPENDENCIES_HOME/Ogre/OSXBuild/lib/Release/Ogre.framework/Versions/1.7.1/Ogre" "$APP_BUNDLE_NAME/Contents/Frameworks/Ogre.framework/Versions/1.7.1/Ogre"
cp "$DEPENDENCIES_HOME/Ogre/OSXBuild/Dependencies/Cg.framework/Versions/1.0/Cg" "$APP_BUNDLE_NAME/Contents/Frameworks/Cg.framework/Cg"

#Copy mono framework
cp -r "$MONO_FRAMEWORK_HOME/Mono.framework" "$APP_BUNDLE_NAME/Contents/Frameworks/Mono.framework"

#Copy override
cp ../override.ini "$APP_BUNDLE_NAME/Contents/Resources"

#Finish up
cd "$START_PATH"