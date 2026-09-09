Document


POST
/documents

Parameters
Cancel
Reset
No parameters

Request body

multipart/form-data
File
string($binary)
Ayush raj.txt
Send empty value
Execute
Clear
Responses
Curl

curl -X 'POST' \
  'http://localhost:5144/documents' \
  -H 'accept: text/plain' \
  -H 'Content-Type: multipart/form-data' \
  -F 'File=@Ayush raj.txt;type=text/plain'
Request URL
http://localhost:5144/documents
Server response
Code	Details
201
Undocumented
Response body
Download
{
  "id": "9be45529-bd9a-4a67-8076-c90ae7711f1d",
  "fileName": "Ayush raj.txt",
  "size": 9,
  "contentType": "text/plain",
  "uploadedAt": "2026-09-09T07:31:39.4642237Z",
  "status": "Uploaded",
  "filePath": "C:\\Users\\ayush\\OneDrive\\Desktop\\Project\\DocMind\\src\\Kastle.DocMind.Api\\Storage\\5b2f0cb7-d219-4720-bdd0-10a600b0e389_Ayush raj.txt"
}
Response headers
 content-type: application/json; charset=utf-8 
 date: Wed,09 Sep 2026 07:31:39 GMT 
 location: http://localhost:5144/documents/9be45529-bd9a-4a67-8076-c90ae7711f1d 
 server: Kestrel 
 transfer-encoding: chunked 
Responses
Code	Description	Links
200	
OK

Media type

text/plain
Controls Accept header.
Example Value
Schema
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "fileName": "string",
  "size": 0,
  "contentType": "string",
  "uploadedAt": "2026-09-09T07:31:39.575Z",
  "status": "string",
  "filePath": "string"
}