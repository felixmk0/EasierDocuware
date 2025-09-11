using DocuWare.Platform.ServerClient;
using EasierDocuware.Models.Documents;
using EasierDocuware.Models.Global;

namespace EasierDocuware.Interfaces.Internal
{
    public interface IDocumentServiceInternal
    {
        Task<ServiceResult<bool>> BatchUpdateDocFieldsAsync(string fileCabinetId, List<int> documentIds, Dictionary<string, string> fields, bool forceUpdate);
        Task<ServiceResult<bool>> BatchUpdateKeywordIndexFieldsAsync(string fileCabinetId, List<int> documentIds, string keywordsFieldName, List<string> keywordValues, string storeDialogId, bool forceUpdate);
        Task<ServiceResult<List<DocumentIndexField>>> GetDocFieldsAsync(Document document);
        Task<ServiceResult<List<DocumentIndexField>>> GetDocFieldsAsync(string fileCabinetId, int docId);
        Task<ServiceResult<Document>> GetDocumentByIdAsync(string fileCabinetId, int docId);
        Task<ServiceResult<List<Document>>> GetDocumentsByFileCabinetIdAsync(string fileCabinetId, int? count = 10000);
        Task<ServiceResult<bool>> UpdateDocFieldsAsync(Document doc, Dictionary<string, string> fields, bool forceUpdate);
        Task<ServiceResult<DocumentFileDownloadResult>> DownloadDocumentAsync(string fileCabinetId, int docId);
        Task<ServiceResult<FileStream>> ExportDocumentAsDwxAsync(string fileCabinetId, int docId, ExportSettings exportSettings);
        Task<ServiceResult<List<int>>> ImportDwxDocAsync(string fileCabinetId, Stream dwxStream, string fileName);
        Task<ServiceResult<bool>> ValidateDocDateFieldAsync(string fileCabinetId, int docId, string dateFieldName);
        Task<ServiceResult<List<Document>>> GetDuplicateDocsAsync(string fileCabinetId, Dictionary<string, object> fields);
        Task<ServiceResult<List<int>>> GetDuplicateDocsIdsAsync(string fileCabinetId, Dictionary<string, object> fields);
    }
}