using DocuWare.Platform.ServerClient;
using EasierDocuware.Interfaces.Internal;
using EasierDocuware.Interfaces.Public;
using EasierDocuware.Models;


namespace EasierDocuware.Services.Internal
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentServiceInternal _documentServiceInternal;

        public DocumentService(IDocumentServiceInternal documentServiceInternal)    
        {
            _documentServiceInternal = documentServiceInternal;
        }


        public Task<ServiceResult<bool>> UpdateDocFieldsAsync(Document doc, Dictionary<string, string> fields, bool forceUpdate) => _documentServiceInternal.UpdateDocFieldsAsync(doc, fields, forceUpdate);
        public Task<ServiceResult<bool>> BatchUpdateDocFieldsAsync(string fileCabinetId, List<int> documentIds, Dictionary<string, string> fields, bool forceUpdate) => _documentServiceInternal.BatchUpdateDocFieldsAsync(fileCabinetId, documentIds, fields, forceUpdate);
        public Task<ServiceResult<bool>> BatchUpdateKeywordIndexFieldsAsync(string fileCabinetId, List<int> documentIds, string keywordsFieldName, List<string> keywordValues, string storeDialogId, bool forceUpdate) => _documentServiceInternal.BatchUpdateKeywordIndexFieldsAsync(fileCabinetId, documentIds, keywordsFieldName, keywordValues, storeDialogId, forceUpdate);
        public Task<ServiceResult<List<Document>>> GetDocumentsByFileCabinetIdAsync(string fileCabinetId, int? count = 10000) => _documentServiceInternal.GetDocumentsByFileCabinetIdAsync(fileCabinetId, count);
        public Task<ServiceResult<Document>> GetDocumentByIdAsync(string fileCabinetId, int docId) => _documentServiceInternal.GetDocumentByIdAsync(fileCabinetId, docId);
        public Task<ServiceResult<List<DocumentIndexField>>> GetDocFieldsAsync(Document document) => _documentServiceInternal.GetDocFieldsAsync(document);
        public Task<ServiceResult<List<DocumentIndexField>>> GetDocFieldsAsync(string fileCabinetId, int docId) => _documentServiceInternal.GetDocFieldsAsync(fileCabinetId, docId);
        public Task<ServiceResult<DocumentFileDownload>> DownloadDocumentAsync(string fileCabinetId, int docId) => _documentServiceInternal.DownloadDocumentAsync(fileCabinetId, docId);
    }
}