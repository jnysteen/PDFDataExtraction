#!/bin/sh

# Builds the Docker image using the Dockerfile in the current
# directory and names the image "pdf-extraction-web-api"
docker build . -t pdf-extraction-web-api