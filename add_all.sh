#!/bin/sh


rm *.sln
dotnet new sln
find . -type f -name "*.csproj" | while IFS= read -r file
do
    dotnet sln add "$file"
done