#!/bin/sh

# Builds the web API Docker image and names the image "pdf-extraction-web-api"
docker build -f Dockerfile.webapi . -t pdf-extraction-web-api