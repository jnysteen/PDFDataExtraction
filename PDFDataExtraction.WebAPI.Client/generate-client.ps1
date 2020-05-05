$swaggergen=$args[0] # The swaggergen command to run (might be the version installed through brew or a Java expression using the jar-file)
$swaggerurl=$args[1] # The URL to the PDF Data Extraction Swagger JSON

Write-Output "Generating client" 
Invoke-Expression "$($swaggergen) generate -i $swaggerurl -l csharp -c swagger-codegen-generation-config.json -o ."

Write-Output "Cleaning up after client generation"
Remove-Item "PDFDataExtraction.WebAPI.Client.sln"
Remove-Item ".\Api" -Recurse
Remove-Item ".\Client" -Recurse
Remove-Item ".\Model" -Recurse
Move-Item -Path ".\src\PDFDataExtraction.WebAPI.Client\*" -Destination "." -Force
Remove-Item ".\src" -Recurse
Remove-Item ".\.swagger-codegen" -Recurse -Force

Write-Output "Fixing generated client"
(Get-Content .\Api\PDFTextExtractionApi.cs).replace('byte[] _file', 'System.IO.Stream _file') | Set-Content .\Api\PDFTextExtractionApi.cs # Fixes that the generated client has types errors in it

Write-Output "Client generation done!"