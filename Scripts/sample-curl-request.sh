#!/bin/sh

# Sends a HTTP request to the API in the running Docker container
# The request contains the file `./Sample-PDFs/1page-sparse-text.pdf` 
# The response is saved in the file `./curl-request-output.json`
curl -o curl-request-output.json -F file=@./Sample-PDFs/1page-sparse-text.pdf http://localhost:6000/api/PDFTextExtraction/detailed