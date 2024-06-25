#!/bin/bash
dotnet build
sharpie bind \
  -framework ../native/bin/Release/net8.0-ios/MauiFacebookiOS.xcframework/ios-arm64/MauiFacebook.framework \
  -namespace Tonestro.Maui.FacebookSlim.iOS
mv ApiDefinitions.cs Apidefinition.cs
