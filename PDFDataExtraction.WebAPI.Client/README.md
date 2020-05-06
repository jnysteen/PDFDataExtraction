# How to generate a client

1. Install OpenAPI Generator (https://github.com/OpenAPITools/openapi-generator)
2. Install Powershell
3. Use Powershell to run the `generate-client.ps1` script, providing the swagger gen command and Swagger JSON URL as inputs. Example `pwsh generate-client.ps1 openapi-generator http://localhost:4000/swagger/v1/swagger.json`