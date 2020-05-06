$swaggergen=$args[0] # The openapi-generator command to run (might be the version installed through brew or a Java expression using the jar-file)
$swaggerurl=$args[1] # The URL to the PDF Data Extraction Swagger JSON

Write-Output "Generating client" 
Invoke-Expression "$($swaggergen) generate -i $swaggerurl -g csharp-netcore -c client-generation-config.json -o . --skip-validate-spec"

Write-Output "Cleaning up after client generation"
Remove-Item "PDFDataExtraction.WebAPI.Client.sln"
Remove-Item ".\Api" -Recurse
Remove-Item ".\Client" -Recurse
Remove-Item ".\Model" -Recurse
Move-Item -Path ".\src\PDFDataExtraction.WebAPI.Client\*" -Destination "." -Force
Remove-Item ".\src" -Recurse
Remove-Item ".\.openapi-generator" -Recurse -Force

Write-Output "Client generation done!"