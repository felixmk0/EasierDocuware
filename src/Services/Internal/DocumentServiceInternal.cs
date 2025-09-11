using DocuWare.Platform.ServerClient;
using EasierDocuware.Interfaces;
using EasierDocuware.Interfaces.Internal;
using EasierDocuware.Models.Documents;
using EasierDocuware.Models.Global;
using Microsoft.IdentityModel.Tokens;


namespace EasierDocuware.Services.Internal
{
    public class DocumentServiceInternal : IDocumentServiceInternal
    {
        private readonly IAuthServiceInternal _authServiceInternal;
        private readonly IFileCabinetServiceInternal _fileCabinetServiceInternal;

        public DocumentServiceInternal(IAuthServiceInternal authServiceInternal, IFileCabinetServiceInternal fileCabinetServiceInternal)
        {
            _authServiceInternal = authServiceInternal;
            _fileCabinetServiceInternal = fileCabinetServiceInternal;
        }


        public async Task<ServiceResult<List<Document>>> GetDocumentsByFileCabinetIdAsync(string fileCabinetId, int? count = 10000)
        {
            try
            {
                var authResult = _authServiceInternal.IsConnectedAsync();
                if (!authResult.Success) return ServiceResult<List<Document>>.Fail(authResult.Message!);

                var _serviceConnection = authResult.Data!;
                DocumentsQueryResult queryResult = await _serviceConnection.GetFromDocumentsForDocumentsQueryResultAsync(fileCabinetId, count: count).ConfigureAwait(false);

                List<Document> result = new List<Document>();
                await GetAllDocumentsAsync(queryResult, result);


                return ServiceResult<List<Document>>.Ok(result);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<Document>>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<Document>> GetDocumentByIdAsync(string fileCabinetId, int docId)
        {
            try
            {
                var authResult = _authServiceInternal.IsConnectedAsync();
                if (!authResult.Success) return ServiceResult<Document>.Fail(authResult.Message!);
                var _serviceConnection = authResult.Data!;

                var docs = await GetDocumentsByFileCabinetIdAsync(fileCabinetId);
                if (!docs.Success) return ServiceResult<Document>.Fail(docs.Message!);

                var doc = docs.Data!.FirstOrDefault(d => d.Id == docId);
                if (doc == null) return ServiceResult<Document>.Fail("Document not found.");

                return ServiceResult<Document>.Ok(doc);
            }
            catch (Exception ex)
            {
                return ServiceResult<Document>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<bool>> UpdateDocFieldsAsync(Document doc, Dictionary<string, string> fields, bool forceUpdate)
        {
            try
            {
                if (doc == null || fields.IsNullOrEmpty()) return ServiceResult<bool>.Fail("Parameteres cannot be null!");

                var docFields = new List<DocumentIndexField>();
                foreach (var field in fields) docFields.Add(DocumentIndexField.Create(field.Key, field.Value));

                var updateIndexFieldsInfo = new UpdateIndexFieldsInfo()
                {
                    Field = docFields,
                    ForceUpdate = forceUpdate
                };

                var response = await doc.PutToFieldsRelationForDocumentIndexFieldsAsync(updateIndexFieldsInfo);
                if (!response.IsSuccessStatusCode) return ServiceResult<bool>.Fail($"Update failed with status code {response.StatusCode}: {response.Exception.Message}");

                return ServiceResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<bool>> BatchUpdateDocFieldsAsync(string fileCabinetId, List<int> documentIds, Dictionary<string, string> fields, bool forceUpdate)
        {
            try
            {
                if (documentIds.IsNullOrEmpty() || fields.IsNullOrEmpty() || fileCabinetId.IsNullOrEmpty()) return ServiceResult<bool>.Fail("Parameteres cannot be null!");

                var docFields = new List<DocumentIndexField>();
                foreach (var field in fields) docFields.Add(DocumentIndexField.Create(field.Key, field.Value));

                var batchUpdateProcess = new BatchUpdateProcess
                {
                    Data = new BatchUpdateProcessData
                    {
                        Field = docFields,
                        ForceUpdate = forceUpdate
                    },
                    Source = new BatchUpdateDocumentsSource
                    {
                        Id = documentIds
                    }
                };

                var result = _fileCabinetServiceInternal.GetFileCabinetById(fileCabinetId);
                if (!result.Success) return ServiceResult<bool>.Fail(result.Message!);
                var fileCabinet = result.Data!;

                var response = await fileCabinet.PostToBatchUpdateRelationForBatchUpdateIndexFieldsResultAsync(batchUpdateProcess);
                if (!response.IsSuccessStatusCode) return ServiceResult<bool>.Fail($"Batch update failed with status code {response.StatusCode}: {response.Exception.Message}");

                return ServiceResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<bool>> BatchUpdateKeywordIndexFieldsAsync(string fileCabinetId, List<int> documentIds, string keywordsFieldName, List<string> keywordValues, string storeDialogId, bool forceUpdate)
        {
            try
            {
                var result = _fileCabinetServiceInternal.GetFileCabinetById(fileCabinetId);
                if (!result.Success) return ServiceResult<bool>.Fail(result.Message!);

                var fileCabinet = result.Data!;
                var batchAppendKeywordValues = new BatchAppendKeywordValues
                {
                    DocId = documentIds,
                    FieldName = keywordsFieldName,
                    Keyword = keywordValues,
                    StoreDialogId = storeDialogId,
                    ForceUpdate = forceUpdate
                };

                var response = await fileCabinet.PostToBatchUpdateRelationForBatchUpdateIndexFieldsResultAsync(batchAppendKeywordValues);
                if (!response.IsSuccessStatusCode) return ServiceResult<bool>.Fail($"Batch append keywords failed with status code {response.StatusCode}: {response.Exception.Message}");

                return ServiceResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Fail(ex.Message);
            }
        }


        public async Task<ServiceResult<List<DocumentIndexField>>> GetDocFieldsAsync(Document document)
        {
            try
            {
                var response = await document.GetDocumentIndexFieldsFromFieldsRelationAsync();
                if (!response.IsSuccessStatusCode) return ServiceResult<List<DocumentIndexField>>.Fail($"Get fields failed with status code {response.StatusCode}: {response.Exception.Message}");

                var docFields = response.Content.Field;
                if (docFields == null) return ServiceResult<List<DocumentIndexField>>.Fail("No fields found for the document.");

                return ServiceResult<List<DocumentIndexField>>.Ok(docFields);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<DocumentIndexField>>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<List<DocumentIndexField>>> GetDocFieldsAsync(string fileCabinetId, int docId)
        {
            try
            {
                var docResult = await GetDocumentByIdAsync(fileCabinetId, docId);
                if (!docResult.Success) return ServiceResult<List<DocumentIndexField>>.Fail(docResult.Message!);
                var document = docResult.Data!;

                var response = await document.GetDocumentIndexFieldsFromFieldsRelationAsync();
                if (!response.IsSuccessStatusCode) return ServiceResult<List<DocumentIndexField>>.Fail($"Get fields failed with status code {response.StatusCode}: {response.Exception.Message}");

                var docFields = response.Content.Field;
                if (docFields == null) return ServiceResult<List<DocumentIndexField>>.Fail("No fields found for the document.");

                return ServiceResult<List<DocumentIndexField>>.Ok(docFields);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<DocumentIndexField>>.Fail(ex.Message);
            }
        }


        public async Task<ServiceResult<DocumentIndexFields>> UpdateTableFieldsAsync(Document document, string fieldName)
        {
            try
            {
                DocumentIndexField tableIndexField = document.Fields.FirstOrDefault(f => f.FieldName == fieldName && f.ItemElementName == ItemChoiceType.Table);

                if (tableIndexField != null && !tableIndexField.IsNull)
                {
                    DocumentIndexFieldTable existingTableField = tableIndexField.Item as DocumentIndexFieldTable;

                    if (existingTableField?.Row.Count > 0)
                    {
                        DocumentIndexFieldTableRow documentIndexFieldTableRow =
                            existingTableField.Row.FirstOrDefault(r =>
                                r.ColumnValue
                                    .Exists(c => c.FieldName == "INVOI_POSITION"
                                        && (decimal)c.Item == 1m));

                        DocumentIndexField columnIndexFieldAmount =
                            documentIndexFieldTableRow?.ColumnValue
                                .FirstOrDefault(c => c.FieldName == "INVOI_AMOUNT");

                        if (columnIndexFieldAmount != null)
                        {
                            //Set for single entry a new price
                            columnIndexFieldAmount.Item = 30m;
                        }
                        else
                        {
                            //Use dedicated exception in production code!
                            throw new Exception("Column not found!");
                        }
                    }
                    else
                    {
                        //Use dedicated exception in production code!
                        throw new Exception("No table field rows available!");
                    }

                }
                else
                {
                    //Use dedicated exception in production code!
                    throw new Exception("Table field does not exist!");
                }

                //Due to reference types we can just take the fields list
                //from original provided document object
                DocumentIndexFields updatedTableIndexFields = new DocumentIndexFields()
                {
                    Field = document.Fields
                };

                //IMPORTANT: Send always ALL table fields to the server.
                //Also in case only a single column value was updated,
                //Otherwise all other table field entries get deleted!
                var result = await document.PutToFieldsRelationForDocumentIndexFieldsAsync(updatedTableIndexFields);
                if (!result.IsSuccessStatusCode) return ServiceResult<DocumentIndexFields>.Fail($"Update failed with status code {result.StatusCode}: {result.Exception.Message}");

                return ServiceResult<DocumentIndexFields>.Ok(updatedTableIndexFields);
            }
            catch (Exception ex)
            {
                return ServiceResult<DocumentIndexFields>.Fail(ex.Message);
            }
        }


        public async Task<ServiceResult<bool>> ValidateDocDateFieldAsync(string fileCabinetId, int docId, string dateFieldName)
        {
            try
            {
                var docResult = await GetDocumentByIdAsync(fileCabinetId, docId);
                if (!docResult.Success) return ServiceResult<bool>.Fail(docResult.Message!);
                var document = docResult.Data!;

                var docFieldsResult = await GetDocFieldsAsync(document);
                if (!docFieldsResult.Success) return ServiceResult<bool>.Fail(docFieldsResult.Message!);
                var docFields = docFieldsResult.Data!;

                var dateField = docFields.FirstOrDefault(f => f.FieldName == dateFieldName);
                if (dateField == null) return ServiceResult<bool>.Fail("Date field not found in the document.");

                var dateValue = dateField.Item as DateTime?;
                if (dateValue == null || !dateValue.HasValue) return ServiceResult<bool>.Fail("Date field value is null or not a valid date.");
                var currentDate = DateTime.Now;
                if (dateValue.Value.Date > currentDate.Date) return ServiceResult<bool>.Fail("The date field value is in the future.");

                return ServiceResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<List<Document>>> GetDuplicateDocsAsync(string fileCabinetId, Dictionary<string, object> fields)
        {
            try
            {
                var docsResult = await GetDocumentsByFileCabinetIdAsync(fileCabinetId);
                if (!docsResult.Success) return ServiceResult<List<Document>>.Fail(docsResult.Message!);

                var docs = docsResult.Data!;
                var duplicateDocs = new List<Document>();

                foreach (var doc in docs)
                {
                    bool matchesAll = true;
                    foreach (var field in fields)
                    {
                        bool hasMatch = doc.Fields.Any(f => f.FieldName == field.Key && f.Item == field.Value);
                        if (!hasMatch)
                        {
                            matchesAll = false;
                            break;
                        }
                    }

                    if (matchesAll) duplicateDocs.Add(doc);
                }

                return ServiceResult<List<Document>>.Ok(duplicateDocs);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<Document>>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<List<int>>> GetDuplicateDocsIdsAsync(string fileCabinetId, Dictionary<string, object> fields)
        {
            try
            {
                var duplicateDocs = await GetDuplicateDocsAsync(fileCabinetId, fields);
                if (!duplicateDocs.Success) return ServiceResult<List<int>>.Fail(duplicateDocs.Message!);

                var docs = duplicateDocs.Data!;
                var duplicateDocsIds = new List<int>();

                docs.ForEach(doc =>
                {
                    duplicateDocsIds.Add(doc.Id);
                });

                return ServiceResult<List<int>>.Ok(duplicateDocsIds);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<int>>.Fail(ex.Message);
            }
        }


        public async Task<ServiceResult<DocumentFileDownloadResult>> DownloadDocumentAsync(string fileCabinetId, int docId)
        {
            try
            {
                var authResult = _authServiceInternal.IsConnectedAsync();
                if (!authResult.Success) return ServiceResult<DocumentFileDownloadResult>.Fail(authResult.Message!);
                var connection = authResult.Data!;

                var documentResponse = await connection.GetFromDocumentForDocumentAsync(docId, fileCabinetId);
                if (!documentResponse.IsSuccessStatusCode) return ServiceResult<DocumentFileDownloadResult>.Fail($"Failed to get document. Status code: {documentResponse.StatusCode}, Message: {documentResponse.Exception.Message}");
                var document = documentResponse.Content;

                var documentDownloadResponse = await DownloadDocumentContentAsync(document);
                if (!documentDownloadResponse.Success) return ServiceResult<DocumentFileDownloadResult>.Fail(documentDownloadResponse.Message!);
                var downloadedFile = documentDownloadResponse.Data!;

                using (var file = File.Create(downloadedFile.FileName))
                using (var stream = downloadedFile.Stream) await stream.CopyToAsync(file);

                return ServiceResult<DocumentFileDownloadResult>.Ok(downloadedFile);
            }
            catch (Exception ex)
            {
                return ServiceResult<DocumentFileDownloadResult>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<FileStream>> ExportDocumentAsDwxAsync(string fileCabinetId, int docId, ExportSettings exportSettings)
        {
            try
            {
                var docResult = await GetDocumentByIdAsync(fileCabinetId, docId);
                if (!docResult.Success) return ServiceResult<FileStream>.Fail(docResult.Message!);
                var doc = docResult.Data!;

                string fileName = Guid.NewGuid().ToString() + ".dwx";
                var fs = new FileStream(fileName, FileMode.OpenOrCreate);

                using (var downloadResponse = await doc.PostToDownloadAsArchiveRelationForStreamAsync(exportSettings))
                {
                    if (!downloadResponse.IsSuccessStatusCode) return ServiceResult<FileStream>.Fail($"Download failed with status code {downloadResponse.StatusCode}: {downloadResponse.Exception.Message}");
                    var download = downloadResponse.Content;

                    await download.CopyToAsync(fs);
                }

                return ServiceResult<FileStream>.Ok(fs);
            }
            catch (Exception ex)
            {
                return ServiceResult<FileStream>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<List<int>>> ImportDwxDocAsync(string fileCabinetId, Stream dwxStream, string fileName)
        {
            string tempPath = Path.Combine(Path.GetTempPath(), fileName);

            try
            {
                await using (var fileStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write))
                {
                    await dwxStream.CopyToAsync(fileStream);
                }

                var fileCabinetResult = _fileCabinetServiceInternal.GetFileCabinetById(fileCabinetId);
                if (!fileCabinetResult.Success) return ServiceResult<List<int>>.Fail(fileCabinetResult.Message!);
                var fileCabinet = fileCabinetResult.Data!;

                var importResult = fileCabinet.ImportArchive(new FileInfo(tempPath));
                if (importResult == null) return ServiceResult<List<int>>.Fail("Import failed, no result returned.");

                var errors = importResult.Results.Where(r => r.Status == ImportEntryStatus.Failed).Select((r, index) => $"Document {index + 1}: {r.ErrorMessage}").ToList();
                if (errors.Any())
                {
                    string errorMessages = string.Join("; ", errors);
                    return ServiceResult<List<int>>.Fail($"Import completed with errors: {errorMessages}");
                }

                var successCount = importResult.Results.Count(r => r.Status == ImportEntryStatus.Succeeded);
                if (successCount == 0) return ServiceResult<List<int>>.Fail("Import failed, no documents were imported.");

                var importedDocs = importResult.Results.Where(r => r.Status == ImportEntryStatus.Succeeded).SelectMany(r => r.EntryVersions).ToList();
                var importedDocsIds = importedDocs.Select(d => d.Id).ToList();

                return ServiceResult<List<int>>.Ok(importedDocsIds);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<int>>.Fail(ex.Message);
            }
            finally
            {
                if (File.Exists(tempPath)) File.Delete(tempPath);
            }
        }


        private static async Task<ServiceResult<List<Document>>> GetAllDocumentsAsync(DocumentsQueryResult queryResult, List<Document> documents)
        {
            try
            {
                documents.AddRange(queryResult.Items);

                if (queryResult.NextRelationLink != null)
                {
                    var queryResultResponse = await queryResult.GetDocumentsQueryResultFromNextRelationAsync();

                    if (!queryResultResponse.IsSuccessStatusCode)
                    {
                        return ServiceResult<List<Document>>.Fail(
                            $"Failed to get next page of documents. " + $"Status code: {queryResultResponse.StatusCode}, " + $"Message: {queryResultResponse.Exception?.Message}"
                        );
                    }

                    return await GetAllDocumentsAsync(queryResultResponse, documents);
                }

                return ServiceResult<List<Document>>.Ok(documents);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<Document>>.Fail(ex.Message);
            }
        }

        private static async Task<ServiceResult<DocumentFileDownloadResult>> DownloadDocumentContentAsync(Document document)
        {
            try
            {
                if (document.FileDownloadRelationLink == null)
                {
                    var docResponse = await document.GetDocumentFromSelfRelationAsync().ConfigureAwait(false);
                    if (!docResponse.IsSuccessStatusCode) return ServiceResult<DocumentFileDownloadResult>.Fail($"Failed to refresh document. Status code: {docResponse.StatusCode}, Message: {docResponse.Exception.Message}");
                    document = docResponse.Content;
                }

                var downloadResponse = await document.PostToFileDownloadRelationForStreamAsync(
                    new FileDownload()
                    {
                        TargetFileType = FileDownloadType.Auto
                    });

                if (!downloadResponse.IsSuccessStatusCode) return ServiceResult<DocumentFileDownloadResult>.Fail($"Download failed with status code {downloadResponse.StatusCode}: {downloadResponse.Exception.Message}");

                var contentHeaders = downloadResponse.ContentHeaders;
                var documentDownload = new DocumentFileDownloadResult()
                {
                    Stream = downloadResponse.Content,
                    ContentLength = contentHeaders.ContentLength,
                    ContentType = contentHeaders.ContentType!.MediaType!,
                    FileName = contentHeaders.ContentDisposition!.FileName!
                };

                return ServiceResult<DocumentFileDownloadResult>.Ok(documentDownload);
            }
            catch (Exception ex)
            {
                return ServiceResult<DocumentFileDownloadResult>.Fail(ex.Message);
            }
        }
    }
}