#!/usr/bin/env bash
#
# For Xamarin Android or iOS, change the version name located in AndroidManifest.xml and Info.plist. 
# AN IMPORTANT THING: YOU NEED DECLARE VERSION_NAME ENVIRONMENT VARIABLE IN APP CENTER BUILD CONFIGURATION.

if [ -z "$MAJOR" ]
then
    echo "You need define the MAJOR variable in App Center"
    exit
fi

if [ -z "$MINOR" ]
then
    echo "You need define the MINOR variable in App Center"
    exit
fi

echo "Looking to update version to '$MAJOR'.'$MINOR'.'$APPCENTER_BUILD_ID'"
echo "'$APPCENTER_SOURCE_DIRECTORY'"

ANDROID_MANIFEST_FILE=$APPCENTER_SOURCE_DIRECTORY/VaxCheckNS.Mobile.Android/Properties/AndroidManifest.xml
INFO_PLIST_FILE=$APPCENTER_SOURCE_DIRECTORY/VaxCheckNS.Mobile.iOS/Info.plist

if [ -e "$ANDROID_MANIFEST_FILE" ]
then
    echo "Updating version name to '$MAJOR'.'$MINOR'.'$APPCENTER_BUILD_ID' in AndroidManifest.xml"
    sed -i '' 's/versionName="[0-9.]*"/versionName="'MAJOR'.'$MINOR'.'$APPCENTER_BUILD_ID'"/' $ANDROID_MANIFEST_FILE

    echo "File content:"
    cat $ANDROID_MANIFEST_FILE
fi


if [ -e "$INFO_PLIST_FILE" ]
then
    echo "Updating version name to '$MAJOR'.'$MINOR'.'$APPCENTER_BUILD_ID' in Info.plist"
    plutil -replace CFBundleShortVersionString -string $VERSION_NAME $INFO_PLIST_FILE
    plutil -replace CFBundleVersion -string "{$MAJOR}.{$MINOR}.{$APPCENTER_BUILD_ID}" $INFO_PLIST_FILE

    echo "File content:"
    cat $INFO_PLIST_FILE
fi