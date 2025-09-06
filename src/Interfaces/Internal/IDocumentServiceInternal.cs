using DocuWare.Platform.ServerClient;
using EasierDocuware.Models;

namespace EasierDocuware.Interfaces.Internal
{
    public interface IDocumentServiceInternal
    {
        Task<ServiceResult<bool>> BatchUpdateDocFieldsAsync(string fileCabinetId, List<int> documentIds, Dictionary<string, string> fields, bool forceUpdate);
        Task<ServiceResult<bool>> ForceUpdateBatchAppendKeywordIndexFields(string fileCabinetId, List<int> documentIds, string keywordsFieldName, List<string> keywordValues, string storeDialogId, bool forceUpdate);
        Task<ServiceResult<List<Document>>> GetDocumentsByFileCabinetIdAsync(string fileCabinetId, int? count = 10000);
        Task<ServiceResult<bool>> UpdateDocFieldsAsync(Document doc, Dictionary<string, string> fields, bool forceUpdate);
    }
}