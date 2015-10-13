#!/bin/sh

# BuildBundle.sh
# 
#
# Created by Andrew Piper on 1/18/11.
# Copyright 2011 Anomalous Software. All rights reserved.

THIS_FOLDER=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )

pushd $THIS_FOLDER

codesign -s "Developer ID Application" ../../../../PublicRelease/Anomalous\ Medical.app --deep

sh ../MakeDMG.sh "Anomalous Medical" "../../../../PublicRelease" "Anomalous Medical" "../Layout" "../../License"

rm AnomalousMedical.dmg

mv Anomalous\ Medical.dmg AnomalousMedical.dmg

popd