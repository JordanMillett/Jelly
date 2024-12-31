#!/bin/bash
pkill Jelly

git pull origin main

dotnet build
dotnet run

# chmod +x update-server.sh