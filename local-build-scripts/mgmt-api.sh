#!/bin/bash

export SONA_BACKEND_VERSION=v2.0.393

find . \( -name "*.config" -o -name "*.csproj" -o -name "*.sln" -o -name "NuGet.config" -o -name "*.json" \) -print0 | tar -cvf projectfiles.tar --null -T -
docker build  -t sonatribe/sonatribeticketmanagementapi:$SONA_BACKEND_VERSION --target management-api .