#!/usr/bin/env bash

echo "WE ARE STARTING SCRIPT"

echo "$PWD"

echo $APPCENTER_SOURCE_DIRECTORY

AppSettingFile=$APPCENTER_SOURCE_DIRECTORY/src/VaxCheckNS.Mobile/VaxCheckNS.Mobile/Helpers/AppSettings.cs

echo $AppSettingFile


if [ -z "$AppCenterAndroidKey" ]
then
    echo "You need define the AppSettingFile variable in App Center - AppCenterAndroidKey"
    exit
fi

if [ -z "$AppCenteriOSKey" ]
then
    echo "You need define the AppSettingFile variable in App Center - AppCenteriOSKey"
    exit
fi

if [ -e "$AppSettingFile" ]
then
    echo "Updating AppCenter Key to $AppCenterAndroidKey in AppSetting.cs"
    sed -i '' 's#AppCenterAndroidKey = "[-A-Za-z0-9:_./]*"#AppCenterAndroidKey = "'$AppCenterAndroidKey'"#' $AppSettingFile

    echo "Updating AppCenter Key to $AppCenteriOSKey in AppSetting.cs"
    sed -i '' 's#AppCenteriOSKey = "[-A-Za-z0-9:_./]*"#AppCenteriOSKey = "'$AppCenteriOSKey'"#' $AppSettingFile

    echo "File content: "
    cat $AppSettingFile
else
    echo "Did not find AppSettings file."
fi

echo "END-SCRIPT"
