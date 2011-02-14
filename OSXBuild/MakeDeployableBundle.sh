#!/bin/sh

# BuildBundle.sh
# 
#
# Created by Andrew Piper on 1/18/11.
# Copyright 2011 Anomalous Software. All rights reserved.

APP_ORIGINAL_NAME=$1
APP_FINAL_NAME=$2

sh ../MakeBundle.sh "$APP_ORIGINAL_NAME" ".."
cp "$APP_ORIGINAL_NAME.dat" "$APP_ORIGINAL_NAME.app/Contents/Resources/$APP_ORIGINAL_NAME.dat"
rm "$APP_ORIGINAL_NAME.app/Contents/Resources/override.ini"

sudo rm -r "$APP_FINAL_NAME.app"
mv "$APP_ORIGINAL_NAME.app" "$APP_FINAL_NAME.app"