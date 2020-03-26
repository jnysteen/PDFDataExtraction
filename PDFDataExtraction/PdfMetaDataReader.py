import pikepdf
from sys import argv
import json

# Opens of the PDF file provided as the first argument, extracts all metadata and prints a JSON dictionary with the extracted
# key/value pairs to stdout 
def main():
  inputfile = argv[1]
  with pikepdf.open(inputfile) as pdf: 
    docinfo = pdf.docinfo.as_dict()
    res = {}
    for attr, value in docinfo.items():
      res[attr] = str(value)
    with open('result.json', 'w') as fp:
      # json.dump(res, fp)
      print(json.dumps(res))
  
if __name__== "__main__":
  main()
