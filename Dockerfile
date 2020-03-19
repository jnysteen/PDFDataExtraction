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



# Install gcc and other tools needed for pdf2text
RUN apk add build-base

# Install pdftotext
RUN apk add --no-cache poppler-utils 

ENV PYTHONUNBUFFERED=1

RUN echo "**** install Python ****" && \
    apk add --no-cache python3 && \
    if [ ! -e /usr/bin/python ]; then ln -sf python3 /usr/bin/python ; fi && \
    \
    echo "**** install pip ****" && \
    python3 -m ensurepip && \
    rm -r /usr/lib/python*/ensurepip && \
    pip3 install --no-cache --upgrade pip setuptools wheel && \
    if [ ! -e /usr/bin/pip ]; then ln -s pip3 /usr/bin/pip ; fi

# Install pdf2text (Text extraction with Python)
RUN pip3 install pdfminer

WORKDIR /app
COPY --from=build /app/PDFDataExtraction.WebAPI/out ./
ENTRYPOINT ["dotnet", "PDFDataExtraction.WebAPI.dll"]