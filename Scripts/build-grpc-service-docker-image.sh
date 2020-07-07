#!/bin/sh

# Builds the web API Docker image and names the image "pdf-extraction-web-api"
docker build -f Dockerfile.grpcservice . -t pdf-extraction-grpcservice