#!/bin/sh

#Delete old app
rm -r -f "Piper JBO.app"

#Compile native libs
xcodebuild -project OSHelper/OSHelper.xcodeproj -alltargets

#Do the macpack
macpack -n "Piper JBO" -a DotNetAssemblies/PiperJBO.exe -m cocoa -i Skull.icns

#Copy the .net assemblies
cp DotNetAssemblies/*.dll "Piper JBO.app/Contents/Resources"
cp DotNetAssemblies/PiperJBO.exe "Piper JBO.app/Contents/Resources/Piper JBO.exe"

#Copy the mac native dylibs
cp ../../Engine/OSXBuild/Release/*.dylib "Piper JBO.app/Contents/Resources"

#Copy the Medical native dylibs
cp OSHelper/build/Release/libOSHelper.dylib "Piper JBO.app/Contents/Resources"

#Copy the wx.NET native dylibs
cp ../../Dependencies/wxDotNet/Build/OSXBuild/wxDotNet/build/Release/libwx-c.dylib "Piper JBO.app/Contents/Resources"

#Copy the frameworks
mkdir "Piper JBO.app/Contents/Frameworks"
mkdir "Piper JBO.app/Contents/Frameworks/Cg.framework"
mkdir "Piper JBO.app/Contents/Frameworks/Ogre.framework"
mkdir "Piper JBO.app/Contents/Frameworks/Ogre.framework/Versions"
mkdir "Piper JBO.app/Contents/Frameworks/Ogre.framework/Versions/1.7.1"

cp ../../Dependencies/Ogre/OSXBuild/lib/Release/*.dylib "Piper JBO.app/Contents/Resources"
cp ../../Dependencies/Ogre/OSXBuild/lib/Release/Ogre.framework/Versions/1.7.1/Ogre "Piper JBO.app/Contents/Frameworks/Ogre.framework/Versions/1.7.1/Ogre"
cp ../../Dependencies/Ogre/OSXBuild/Dependencies/Cg.framework/Versions/1.0/Cg "Piper JBO.app/Contents/Frameworks/Cg.framework/Cg"

#Create dummy files
mkdir "Piper JBO.app/Contents/Resources/bin"
mkdir "Piper JBO.app/Contents/Resources/bin/Contents"
mkdir "Piper JBO.app/Contents/Resources/bin/Contents/Plugins"

#Copy override
cp override.ini "Piper JBO.app/Contents/Resources"