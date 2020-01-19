# Publishing on Android
I'm too lazy right now to figure out the command line version of this.

To publish through Visual Studio.
1. Open MedicalAndroid.sln
2. Update the Version number and Version name under AnomalousMedicalAndroid->Properties->Android Manifest.
3. Right click on `AnomalousMedicalAndroid` and click `Archive...`.
4. The Archive Manager will open. Let it create the archive. The versions should match what you set above.
5. Once this is complete click Distribute.
6. Click Ad Hoc.
7. If needed import the keystore with the signing key.
8. Choose the key and click Save As.
9. Enter the password for the key store and save the apk to the directory of your choice.
10. You should now have a working Android apk that can be installed on any device.