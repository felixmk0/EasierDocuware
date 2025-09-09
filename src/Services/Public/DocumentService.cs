using DocuWare.Platform.ServerClient;
using EasierDocuware.Interfaces.Public;
using EasierDocuware.Models.Documents;
using EasierDocuware.Models.Global;


namespace EasierDocuware.Services.Internal
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentService _documentService;

        public DocumentService(IDocumentService documentService)    
        {
            _documentService = documentService;
        }


        public Task<ServiceResult<bool>> UpdateDocFieldsAsync(Document doc, Dictionary<string, string> fields, bool forceUpdate) => _documentService.UpdateDocFieldsAsync(doc, fields, forceUpdate);
        public Task<ServiceResult<bool>> BatchUpdateDocFieldsAsync(string fileCabinetId, List<int> documentIds, Dictionary<string, string> fields, bool forceUpdate) => _documentService.BatchUpdateDocFieldsAsync(fileCabinetId, documentIds, fields, forceUpdate);
        public Task<ServiceResult<bool>> BatchUpdateKeywordIndexFieldsAsync(string fileCabinetId, List<int> documentIds, string keywordsFieldName, List<string> keywordValues, string storeDialogId, bool forceUpdate) => _documentService.BatchUpdateKeywordIndexFieldsAsync(fileCabinetId, documentIds, keywordsFieldName, keywordValues, storeDialogId, forceUpdate);
        public Task<ServiceResult<List<Document>>> GetDocumentsByFileCabinetIdAsync(string fileCabinetId, int? count = 10000) => _documentService.GetDocumentsByFileCabinetIdAsync(fileCabinetId, count);
        public Task<ServiceResult<Document>> GetDocumentByIdAsync(string fileCabinetId, int docId) => _documentService.GetDocumentByIdAsync(fileCabinetId, docId);
        public Task<ServiceResult<List<DocumentIndexField>>> GetDocFieldsAsync(Document document) => _documentService.GetDocFieldsAsync(document);
        public Task<ServiceResult<List<DocumentIndexField>>> GetDocFieldsAsync(string fileCabinetId, int docId) => _documentService.GetDocFieldsAsync(fileCabinetId, docId);
        public Task<ServiceResult<DocumentFileDownloadResult>> DownloadDocumentAsync(string fileCabinetId, int docId) => _documentService.DownloadDocumentAsync(fileCabinetId, docId);
        public Task<ServiceResult<FileStream>> ExportDocumentAsDwxAsync(string fileCabinetId, int docId, ExportSettings exportSettings) => _documentService.ExportDocumentAsDwxAsync(fileCabinetId, docId, exportSettings);
        public Task<ServiceResult<List<int>>> ImportDwxDocAsync(string fileCabinetId, Stream dwxStream, string fileName) => _documentService.ImportDwxDocAsync(fileCabinetId, dwxStream, fileName);
    }
}