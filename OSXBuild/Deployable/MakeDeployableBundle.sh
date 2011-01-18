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

#Discover some stuff about where we are.
START_PATH=$(pwd)
MAIN_EXE="$START_PATH/$DOT_NET_EXECUTABLES/$APP_NAME.exe"

#Make sure the temp directories are empty by removing them
rm -r "$BUILD_DIR"
sudo rm -r "$APP_BUNDLE_NAME"

#Setup mkbundle environment vars
export PKG_CONFIG_PATH="/Library/Frameworks/Mono.framework/Versions/Current/lib/pkgconfig/"
export AS="as -arch i386"
export CC="cc -arch i386"
PATH=$PATH:"/Library/Frameworks/Mono.framework/Versions/Current/bin"

#Link with mkbundle
mkdir "$BUILD_DIR"
cd "$BUILD_DIR"
mkbundle "$MAIN_EXE" --deps -c -o "$APP_NAME.c" --keeptemp
cc -arch i386 -g -o "$APP_NAME" -Wall "$APP_NAME.c" `pkg-config --cflags --libs mono-2` temp.o -framework CoreFoundation
cd "$START_PATH"

#Create the app bundle
mkdir "$APP_BUNDLE_NAME"
cp -r "$BUNDLE_SOURCE_DIR/Contents" "$APP_BUNDLE_NAME"
cp "$BUILD_DIR/$APP_NAME" "$APP_BUNDLE_NAME/Contents/Resources"
chmod 755 "$APP_BUNDLE_NAME/Contents/MacOS/PiperJBO"
rm -rf `find $APP_BUNDLE_NAME -type d -name .svn`

#Fix library paths with nant script
sudo nant -buildfile:fixmonopaths.build -D:arg.programName="$APP_NAME"

#Finish up
cd "$START_PATH"
rm -r "$BUILD_DIR"