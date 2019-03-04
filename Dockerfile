FROM mcr.microsoft.com/dotnet/core/sdk:2.1-alpine AS build
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

FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-alpine AS runtime
# Install pdftotext
RUN apk add --no-cache poppler-utils 

# Install pdf2text (Text extraction with Python)
RUN apk add --update \
    python \
    python-dev \
    py-pip \
    build-base \
  && pip install pdfminer \
  && rm -rf /var/cache/apk/*
  
  

WORKDIR /app
COPY --from=build /app/PDFDataExtraction.WebAPI/out ./
ENTRYPOINT ["dotnet", "PDFDataExtraction.WebAPI.dll"]