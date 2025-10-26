#!/bin/bash

if [ $# -ne 1 ]; then
    echo "Usage: $0 [major|minor|patch]"
    exit 1
fi

TYPE=$1
CSPROJ_FILE="src/TaskList/TaskList.csproj"

CURRENT_VERSION=$(grep -oP '(?<=<Version>)[0-9]+\.[0-9]+\.[0-9]+(?=</Version>)' "$CSPROJ_FILE")

if [ -z "$CURRENT_VERSION" ]; then
    echo "Could not find the version in the csproj file."
    exit 1
fi

IFS='.' read -r MAJOR MINOR PATCH <<< "$CURRENT_VERSION"

case $TYPE in
    major)
        ((MAJOR++))
        MINOR=0
        PATCH=0
        ;;
    minor)
        ((MINOR++))
        PATCH=0
        ;;
    patch)
        ((PATCH++))
        ;;
    *)
        echo "Invalid type. Use: major, minor, or patch"
        exit 1
        ;;
esac

NEW_VERSION="$MAJOR.$MINOR.$PATCH"

sed -i "s|<Version>$CURRENT_VERSION</Version>|<Version>$NEW_VERSION</Version>|" "$CSPROJ_FILE"

git add $CSPROJ_FILE
git commit -m $NEW_VERSION
git tag "v$NEW_VERSION"

echo "✅ Version updated from $CURRENT_VERSION to $NEW_VERSION"
