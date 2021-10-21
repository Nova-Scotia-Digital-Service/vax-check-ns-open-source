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

echo "Looking to update version to '$MAJOR.$MINOR.$APPCENTER_BUILD_ID'"

INFO_PLIST_FILE=$APPCENTER_SOURCE_DIRECTORY/VaxCheckNS.Mobile/VaxCheckNS.Mobile.iOS/Info.plist

if [ -e "$INFO_PLIST_FILE" ]
then
    echo "Updating version name to '$MAJOR.$MINOR.$APPCENTER_BUILD_ID' in Info.plist"
    plutil -replace CFBundleShortVersionString -string "$MAJOR.$MINOR.$APPCENTER_BUILD_ID" $INFO_PLIST_FILE
    plutil -replace CFBundleVersion -string "$MAJOR.$MINOR.$APPCENTER_BUILD_ID" $INFO_PLIST_FILE

    echo "File content:"
    cat $INFO_PLIST_FILE
fi