#! /usr/bin/env pwsh
#Requires -Version 7.0
#Requires -PSEdition Core

param (
    [string] $Configuration = "Release",
    [string] $OutputPath = (Join-Path $PSScriptRoot "artifacts")
)


$testProjectPaths = @(
    "tests/ArgumentativeFilters.Tests/ArgumentativeFilters.Tests.csproj"
)

foreach ($testProjectPath in $testProjectPaths) {
    dotnet test $testProjectPath --configuration $Configuration
}

$projectPath = "src/ArgumentativeFilters/ArgumentativeFilters.csproj"

dotnet pack $projectPath --configuration $Configuration --output (Join-Path $OutputPath "packages") --version-suffix "preview"