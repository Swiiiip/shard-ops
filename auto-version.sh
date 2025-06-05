#!/bin/bash
# Auto-versioning script using GitVersion for .NET projects
# Requires: dotnet tool install -g GitVersion.Tool
set -e

# Ensure GitVersion is installed
dotnet tool update -g GitVersion.Tool
export PATH="$PATH:~/.dotnet/tools"

# Run GitVersion to get version info
GITVERSION_JSON=$(GitVersion -output json)
VERSION=$(echo "$GITVERSION_JSON" | grep -Po '"NuGetVersionV2":\s*"\K[^"]+')

# Update all .csproj files with the new version
find . -name '*.csproj' | while read -r csproj; do
  if grep -q '<Version>' "$csproj"; then
    sed -i "s#<Version>.*</Version>#<Version>$VERSION</Version>#" "$csproj"
  else
    sed -i "/<PropertyGroup>/a \\t<Version>$VERSION</Version>" "$csproj"
  fi
done

echo "Updated all .csproj files to version $VERSION"
