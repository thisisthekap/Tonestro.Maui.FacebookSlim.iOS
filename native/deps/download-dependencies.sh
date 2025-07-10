#!/bin/zsh
cd "${0%/*}"
rm -rf *.xcframework
curl "https://github.com/facebook/facebook-ios-sdk/releases/download/v18.0.0/FacebookSDK-Static_XCFramework.zip" -LO
unzip FacebookSDK-Static_XCFramework.zip
mv XCFrameworks/*.xcframework .
rm -d XCFrameworks
rm FacebookSDK-Static_XCFramework.zip
