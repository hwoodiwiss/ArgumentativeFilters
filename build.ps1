#! /usr/bin/env pwsh
#Requires -Version 7.0
#Requires -PSEdition Core

param (
    [string] $Configuration = "Release",
    [string] $OutputPath = (Join-Path $PSScriptRoot "artifacts")
)

$projectPath = "$PSScriptRoot/src/ArgumentativeFilters/ArgumentativeFilters.csproj"

dotnet pack $projectPath -c $Configuration -o (Join-Path $OutputPath "packages")