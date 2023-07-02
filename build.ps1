#! /usr/bin/env pwsh
#Requires -Version 7.0
#Requires -PSEdition Core

param (
    [string] $Configuration = "Release",
    [string] $OutputPath = (Join-Path $PSScriptRoot "artifacts"),
    [switch] $SkipTests = $false
)

$buildProjectPaths = @(
    "examples/ExampleMinimalApi/ExampleMinimalApi.csproj"
)

$testProjectPaths = @(
    "tests/ArgumentativeFilters.Functional.Tests/ArgumentativeFilters.Functional.Tests.csproj"
)

$packageProjectPaths = @(
    "src/ArgumentativeFilters/ArgumentativeFilters.csproj"
)


foreach ($buildProjectPath in $buildProjectPaths) {
    dotnet build $buildProjectPath --configuration $Configuration

    if ($LASTEXITCODE -ne 0) {
        throw "dotnet build failed with exit code $LASTEXITCODE"
    }
}

if (-not $SkipTests) {
    foreach ($testProjectPath in $testProjectPaths) {
        dotnet test $testProjectPath --configuration $Configuration
    
        if ($LASTEXITCODE -ne 0) {
            throw "dotnet test failed with exit code $LASTEXITCODE"
        }
    }
}

foreach ($packageProjectPath in $packageProjectPaths) {
    dotnet pack $packageProjectPath --configuration $Configuration

    if ($LASTEXITCODE -ne 0) {
        throw "dotnet pack failed with exit code $LASTEXITCODE"
    }
}