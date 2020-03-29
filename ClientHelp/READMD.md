# README

## How to generate a client for the PDF Data Extraction Service API

1. Ensure that you have the SDK for .NET Core 2.1 installed (https://dotnet.microsoft.com/download/dotnet-core/2.1).
1. Ensure that the `nswag` CLI tool is installed (https://github.com/RicoSuter/NSwag/wiki/NSwagStudio)
1. Download the new `swagger.json` file from the PDF Data Extraction Service
1. Run `nswag run` in this directory
1. Use the generated client (found in the outputted `.cs` file) to interact with the service! 

