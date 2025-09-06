using DocuWare.Platform.ServerClient;
using EasierDocuware.Interfaces.Internal;
using EasierDocuware.Interfaces.Public;
using EasierDocuware.Models;


namespace EasierDocuware.Services.Internal
{
    /// <summary>
    /// Service for handling DocuWare documents.
    /// Acts as a wrapper around the internal document service.
    /// </summary>
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentServiceInternal _documentServiceInternal;

        public DocumentService(IDocumentServiceInternal documentServiceInternal)
        {
            _documentServiceInternal = documentServiceInternal;
        }

        /// <summary>
        /// Updates the fields of a single document asynchronously.
        /// </summary>
        /// <param name="doc">The document to update.</param>
        /// <param name="fields">Dictionary of field names and values to update.</param>
        /// <param name="forceUpdate">If true, forces the update even if the field is unchanged.</param>
        /// <returns>A <see cref="ServiceResult{T}"/> indicating success or failure of the operation.</returns>
        public Task<ServiceResult<bool>> UpdateDocFieldsAsync(Document doc, Dictionary<string, string> fields, bool forceUpdate)
        {
            return _documentServiceInternal.UpdateDocFieldsAsync(doc, fields, forceUpdate);
        }

        /// <summary>
        /// Updates multiple documents' fields in batch asynchronously.
        /// </summary>
        /// <param name="fileCabinetId">The ID of the file cabinet containing the documents.</param>
        /// <param name="documentIds">List of document IDs to update.</param>
        /// <param name="fields">Dictionary of field names and values to update.</param>
        /// <param name="forceUpdate">If true, forces the update even if the field is unchanged.</param>
        /// <returns>A <see cref="ServiceResult{T}"/> indicating success or failure of the operation.</returns>
        public Task<ServiceResult<bool>> BatchUpdateDocFieldsAsync(string fileCabinetId, List<int> documentIds, Dictionary<string, string> fields, bool forceUpdate)
        {
            return _documentServiceInternal.BatchUpdateDocFieldsAsync(fileCabinetId, documentIds, fields, forceUpdate);
        }

        /// <summary>
        /// Appends keyword index fields to multiple documents and optionally forces update.
        /// </summary>
        /// <param name="fileCabinetId">The ID of the file cabinet containing the documents.</param>
        /// <param name="documentIds">List of document IDs to update.</param>
        /// <param name="keywordsFieldName">The name of the keyword index field.</param>
        /// <param name="keywordValues">List of keyword values to append.</param>
        /// <param name="storeDialogId">The store dialog ID used for the update.</param>
        /// <param name="forceUpdate">If true, forces the update even if the field is unchanged.</param>
        /// <returns>A <see cref="ServiceResult{T}"/> indicating success or failure of the operation.</returns>
        public Task<ServiceResult<bool>> BatchUpdateKeywordIndexFieldsAsync(string fileCabinetId, List<int> documentIds, string keywordsFieldName, List<string> keywordValues, string storeDialogId, bool forceUpdate)
        {
            return _documentServiceInternal.BatchUpdateKeywordIndexFieldsAsync(fileCabinetId, documentIds, keywordsFieldName, keywordValues, storeDialogId, forceUpdate);
        }

        /// <summary>
        /// Retrieves documents from a file cabinet asynchronously.
        /// </summary>
        /// <param name="fileCabinetId">The ID of the file cabinet.</param>
        /// <param name="count">Optional maximum number of documents to retrieve (default 10000).</param>
        /// <returns>A <see cref="ServiceResult{T}"/> containing a <see cref="List{Document}"/> or an error message.</returns>
        public Task<ServiceResult<List<Document>>> GetDocumentsByFileCabinetIdAsync(string fileCabinetId, int? count = 10000)
        {
            return _documentServiceInternal.GetDocumentsByFileCabinetIdAsync(fileCabinetId, count);
        }
    }
}
