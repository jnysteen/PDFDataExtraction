if [ -z "$1" ]; then
    echo "No input file provided!"
    exit
fi

if [ -z "$2" ]; then
    echo "No output file path provided!"
    exit
fi

curl -v --output $2 -F file=@$1 http://localhost:6000/api/PDFTextExtraction/detailed
