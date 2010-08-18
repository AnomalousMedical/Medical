#!/bin/sh

#Delete old app
rm -r -f Articulometrics.app

#Compile native libs
xcodebuild -project OSHelper/OSHelper.xcodeproj -alltargets

#Do the macpack
macpack -n Articulometrics -a DotNetAssemblies/Standalone.exe -m cocoa -i Skull.icns

#Copy the .net assemblies
cp DotNetAssemblies/*.dll Articulometrics.app/Contents/Resources
cp DotNetAssemblies/Standalone.exe Articulometrics.app/Contents/Resources/Articulometrics.exe

#Copy the mac native dylibs
cp ../../Engine/OSXBuild/Release/*.dylib Articulometrics.app/Contents/Resources

#Copy the Medical native dylibs
cp OSHelper/build/Release/libOSHelper.dylib Articulometrics.app/Contents/Resources

#Copy the wx.NET native dylibs
cp ../../Dependencies/wxDotNet/Build/OSXBuild/wxDotNet/build/Release/libwx-c.dylib Articulometrics.app/Contents/Resources

#Copy the frameworks
mkdir Articulometrics.app/Contents/Frameworks
mkdir Articulometrics.app/Contents/Frameworks/Cg.framework
mkdir Articulometrics.app/Contents/Frameworks/Ogre.framework
mkdir Articulometrics.app/Contents/Frameworks/Ogre.framework/Versions
mkdir Articulometrics.app/Contents/Frameworks/Ogre.framework/Versions/1.7.1

cp ../../Dependencies/Ogre/OSXBuild/lib/Release/*.dylib Articulometrics.app/Contents/Resources
cp ../../Dependencies/Ogre/OSXBuild/lib/Release/Ogre.framework/Versions/1.7.1/Ogre Articulometrics.app/Contents/Frameworks/Ogre.framework/Versions/1.7.1/Ogre
cp ../../Dependencies/Ogre/OSXBuild/Dependencies/Cg.framework/Versions/1.0/Cg Articulometrics.app/Contents/Frameworks/Cg.framework/Cg

#Create dummy files
mkdir Articulometrics.app/Contents/Resources/bin
mkdir Articulometrics.app/Contents/Resources/bin/Contents
mkdir Articulometrics.app/Contents/Resources/bin/Contents/Plugins

#Copy override
cp override.ini Articulometrics.app/Contents/Resources