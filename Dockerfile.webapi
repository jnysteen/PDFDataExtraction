# Could probably build a much smaller image by following this: https://coderbook.com/@marcus/how-to-optimize-docker-images-for-smaller-size-and-speed/

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY PDFDataExtraction.WebAPI/*.csproj ./PDFDataExtraction.WebAPI/
COPY PDFDataExtraction/*.csproj ./PDFDataExtraction/
WORKDIR /app/PDFDataExtraction.WebAPI
RUN dotnet restore

# copy and publish app and libraries
WORKDIR /app/
COPY PDFDataExtraction.WebAPI/. ./PDFDataExtraction.WebAPI/
COPY PDFDataExtraction/. ./PDFDataExtraction/
WORKDIR /app/PDFDataExtraction.WebAPI
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS runtime

# install System.Drawing native dependencies
RUN apt-get update \
    && apt-get install -y --allow-unauthenticated --no-install-recommends \
        libc6-dev \ 
        libgdiplus \
        libx11-dev \
        python3 \
        python3-pip \
        ghostscript \
        && \
    rm -rf /var/lib/apt/lists/*

ENV PYTHONUNBUFFERED=1
RUN echo "**** setup Python ****" && \    
    if [ ! -e /usr/bin/python ]; then ln -sf python3 /usr/bin/python ; fi && \
    echo "**** setup pip ****" && \
    pip3 install --upgrade pip setuptools wheel && \
    if [ ! -e /usr/bin/pip ]; then ln -s pip3 /usr/bin/pip ; fi

# Install pdf2text (Text extraction with Python) AND pikepdf (PDF meta data extraction)
RUN pip3 install pdfminer && \ 
    pip3 install pikepdf

RUN apt-get remove -y python3-pip && apt-get clean

WORKDIR /app
COPY --from=build /app/PDFDataExtraction.WebAPI/out ./
CMD ["dotnet", "PDFDataExtraction.WebAPI.dll"]