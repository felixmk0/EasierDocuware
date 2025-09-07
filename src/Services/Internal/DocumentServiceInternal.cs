using DocuWare.Platform.ServerClient;
using EasierDocuware.Interfaces;
using EasierDocuware.Interfaces.Internal;
using EasierDocuware.Models;
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


        // FALTA IMPLEMENTAR GESTIÓ D ERRORS AMB LA REPOSTA HTTP DE DW
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

        public async Task<ServiceResult<Document>> GetDocumentByIdAsync(string fileCabinetId, string docId)
        {
            try
            {
                var authResult = _authServiceInternal.IsConnectedAsync();
                if (!authResult.Success) return ServiceResult<Document>.Fail(authResult.Message!);
                var _serviceConnection = authResult.Data!;

                var docs = await GetDocumentsByFileCabinetIdAsync(fileCabinetId);
                if (!docs.Success) return ServiceResult<Document>.Fail(docs.Message!);

                var doc = docs.Data!.FirstOrDefault(d => d.Id.ToString() == docId);
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

        public async Task<ServiceResult<List<DocumentIndexField>>> GetDocFieldsAsync(string fileCabinetId, string docId)
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


        // UTILITZA EL NOM INTERN DEL CAMP/COLUMNA DE LA DB)
        // FALTA CANBIAR I MILLOR UTILITZAR DIALOG EXPRESSION
        /*
        public async Task<ServiceResult<List<Document>>> SearchDocFieldsAsync(string fileCabinetId, Dictionary<string, object> fields)
        {
            try
            {
                var fileCabinetDocsResult = await GetDocumentsByFileCabinetIdAsync(fileCabinetId);
                if (!fileCabinetDocsResult.Success) return ServiceResult<List<Document>>.Fail(fileCabinetDocsResult.Message!);

                var docs = fileCabinetDocsResult.Data!;

                var matchedDocs = docs.Where(d => fields.All(f =>
                    d.Fields.Any(df => df.FieldName == f.Key && df.Item == f.Value)
                )).ToList();

                if (matchedDocs.IsNullOrEmpty()) return ServiceResult<List<Document>>.Fail("No documents matched the search criteria.");

                return ServiceResult<List<Document>>.Ok(matchedDocs);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<Document>>.Fail(ex.Message);
            }
        }
        */


        private async Task GetAllDocumentsAsync(DocumentsQueryResult queryResult, List<Document> documents)
        {
            documents.AddRange(queryResult.Items);
            if (queryResult.NextRelationLink != null) await GetAllDocumentsAsync(await queryResult.GetDocumentsQueryResultFromNextRelationAsync(), documents);
        }
    }
}