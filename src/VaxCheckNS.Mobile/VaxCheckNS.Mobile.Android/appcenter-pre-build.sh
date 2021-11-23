

echo "$pwd"

echo $APPCENTER_SOURCE_DIRECTORY

echo "Looking to update version to '$MAJOR.$MINOR.$APPCENTER_BUILD_ID'"

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



ANDROID_MANIFEST_FILE=$APPCENTER_SOURCE_DIRECTORY/VaxPassPEI.Mobile/VaxPassPEI.Mobile.Android/Properties/AndroidManifest.xml

if [ -e "$ANDROID_MANIFEST_FILE" ]
then
    echo "Updating version name to '$MAJOR.$MINOR.$APPCENTER_BUILD_ID' in AndroidManifest.xml"
    sed -i '' 's/versionName="[0-9.]*"/versionName="'$MAJOR.$MINOR.$APPCENTER_BUILD_ID'"/' $ANDROID_MANIFEST_FILE
    sed -i '' 's/versionCode="[0-9.]*"/versionCode="'$APPCENTER_BUILD_ID'"/' $ANDROID_MANIFEST_FILE

    echo "File content:"
    cat $ANDROID_MANIFEST_FILE
fi