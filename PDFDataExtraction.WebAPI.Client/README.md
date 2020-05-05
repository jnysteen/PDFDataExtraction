# How to generate a client

1. Install Swagger Codegen (https://github.com/swagger-api/swagger-codegen)
2. Install Powershell
3. Use Powershell to run the `generate-client.ps1` script, providing the swagger gen command and Swagger JSON URL as inputs. Example `pwsh generate-client.ps1 swagger-codegen http://localhost:4000/swagger/v1/swagger.json` 