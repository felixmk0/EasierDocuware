using DocuWare.Platform.ServerClient;
using EasierDocuware.Interfaces.Internal;
using EasierDocuware.Interfaces.Public;
using EasierDocuware.Models.Documents;
using EasierDocuware.Models.Global;


namespace EasierDocuware.Services.Public
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentServiceInternal _internal;

        public DocumentService(IDocumentServiceInternal internalService)
        {
            _internal = internalService;
        }


        public Task<ServiceResult<bool>> UpdateDocFieldsAsync(Document doc, Dictionary<string, string> fields, bool forceUpdate) => _internal.UpdateDocFieldsAsync(doc, fields, forceUpdate);
        public Task<ServiceResult<bool>> BatchUpdateDocFieldsAsync(string fileCabinetId, List<int> documentIds, Dictionary<string, string> fields, bool forceUpdate) => _internal.BatchUpdateDocFieldsAsync(fileCabinetId, documentIds, fields, forceUpdate);
        public Task<ServiceResult<bool>> BatchUpdateKeywordIndexFieldsAsync(string fileCabinetId, List<int> documentIds, string keywordsFieldName, List<string> keywordValues, string storeDialogId, bool forceUpdate) => _internal.BatchUpdateKeywordIndexFieldsAsync(fileCabinetId, documentIds, keywordsFieldName, keywordValues, storeDialogId, forceUpdate);
        public Task<ServiceResult<List<Document>>> GetDocumentsByFileCabinetIdAsync(string fileCabinetId, int? count = 10000) => _internal.GetDocumentsByFileCabinetIdAsync(fileCabinetId, count);
        public Task<ServiceResult<Document>> GetDocumentByIdAsync(string fileCabinetId, int docId) => _internal.GetDocumentByIdAsync(fileCabinetId, docId);
        public Task<ServiceResult<List<DocumentIndexField>>> GetDocFieldsAsync(Document document) => _internal.GetDocFieldsAsync(document);
        public Task<ServiceResult<List<DocumentIndexField>>> GetDocFieldsAsync(string fileCabinetId, int docId) => _internal.GetDocFieldsAsync(fileCabinetId, docId);
        public Task<ServiceResult<DocumentFileDownloadResult>> DownloadDocumentAsync(string fileCabinetId, int docId) => _internal.DownloadDocumentAsync(fileCabinetId, docId);
        public Task<ServiceResult<FileStream>> ExportDocumentAsDwxAsync(string fileCabinetId, int docId, ExportSettings exportSettings) => _internal.ExportDocumentAsDwxAsync(fileCabinetId, docId, exportSettings);
        public Task<ServiceResult<List<int>>> ImportDwxDocAsync(string fileCabinetId, Stream dwxStream, string fileName) => _internal.ImportDwxDocAsync(fileCabinetId, dwxStream, fileName);
        public Task<ServiceResult<bool>> ValidateDocDateFieldAsync(string fileCabinetId, int docId, string dateFieldName) => _internal.ValidateDocDateFieldAsync(fileCabinetId, docId, dateFieldName);
        public Task<ServiceResult<List<Document>>> GetDuplicateDocsAsync(string fileCabinetId, Dictionary<string, object> fields) => _internal.GetDuplicateDocsAsync(fileCabinetId, fields);
        public Task<ServiceResult<List<int>>> GetDuplicateDocsIdsAsync(string fileCabinetId, Dictionary<string, object> fields) => _internal.GetDuplicateDocsIdsAsync(fileCabinetId, fields);
    }
}