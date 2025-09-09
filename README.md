# EasierDocuWare

EasierDocuWare is a .NET library that simplifies working with the [DocuWare Platform SDK](https://developer.docuware.com/).  
It provides a clean, strongly-typed API with XML-documented services, making it easy to connect, search, and manage documents, file cabinets & more. Additionally, EasierDocuWare includes extra features not available in the official SDK.

## Features

- [x] **Authentication**  
  - Connect to DocuWare server with credentials (`ConnectAsync`).
  - Connect to DocuWare via App Registration (`ConnectAppRegistrationAsync`).
  - Disconnect from the DocuWare server (`DisconnectAsync`).
- [x] **Organizations**  
  - Retrieve default organization (`GetOrganization`).  
  - Retrieve all organizations (`GetOrganizationsAsync`).  
- [x] **File Cabinets**  
  - Retrieve all file cabinets (`GetFileCabinetsAsync`).  
  - Retrieve a file cabinet by ID (`GetFileCabinetById`).  
- [x] **Documents**  
  - Retrieve documents from a file cabinet (`GetDocumentsByFileCabinetIdAsync`).
  - Retrieve a document by ID (`GetDocumentByIdAsync`).
  - Retrieve document fields by document (`GetDocFieldsAsync`).
  - Retrieve document fields by file cabinet and document ID (`GetDocFieldsAsync`).
  - Update single document fields (`UpdateDocFieldsAsync`).  
  - Batch update multiple documents (`BatchUpdateDocFieldsAsync`).
  - Batch update multiple documents keyword index fields (`BatchUpdateKeywordIndexFieldsAsync`).
  - Download a document (stream) by document and file cabinet ID (`DownloadDocumentAsync`).
  - Export a document as a DWX file by document and file cabinet ID (`ExportDocumentAsDwxAsync`).
  - Import a DWX document via a stream by file cabinet ID (`ImportDwxDocAsync`).


## Getting Started

### Requirements
- .NET 6.0 or higher
- Access to a DocuWare server (cloud or on-premise)
- Valid credentials

### Installation
Clone the repo and reference the project in your solution:

```bash
git clone https://github.com/felixmk0/EasierDocuware.git
