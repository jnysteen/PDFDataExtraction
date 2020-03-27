# Requires:
# * The CLI tool nswag to be installed: https://github.com/RicoSuter/NSwag/wiki/CommandLine
# * The PDF Data Extraction Service swagger.json to be saved from whatever folder this script is run from

nswag openapi2csclient /input:swagger.json /classname:PdfDataExtractionClient /namespace:PdfDataExtraction /output:PdfDataExtractionClient.cs