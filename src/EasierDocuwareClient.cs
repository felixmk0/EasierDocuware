using DocuWare.Platform.ServerClient;
using EasierDocuware.Interfaces.Public;
using EasierDocuware.Models;


namespace EasierDocuware
{
    /// <summary>
    /// EasierDocuWare main facade.
    /// Provides simple access to authentication, file cabinets, and documents.
    /// </summary>
    public class EasierDocuwareClient
    {
        private readonly IAuthService _authService;
        private readonly IFileCabinetService _fileCabinetService;
        private readonly IDocumentService _documentService;
        private readonly IOrganizationService _organizationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EasierDocuwareClient"/> class.
        /// </summary>
        /// <param name="authService">The authentication service.</param>
        /// <param name="fileCabinetService">The file cabinet service.</param>
        /// <param name="documentService">The document service.</param>
        public EasierDocuwareClient(IAuthService authService, IFileCabinetService fileCabinetService, IDocumentService documentService, IOrganizationService organizationService)
        {
            _authService = authService;
            _fileCabinetService = fileCabinetService;
            _documentService = documentService;
            _organizationService = organizationService;
        }



        ////////////////////////////////////////////////////////////////////
        // AUTHENTICATION
        ////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Connects to the DocuWare server using the provided credentials.
        /// </summary>
        /// <param name="url">The URL of the DocuWare server.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>A <see cref="ServiceResult{bool}"/> indicating success or failure.</returns>
        public Task<ServiceResult<bool>> ConnectAsync(string url, string username, string password) => _authService.ConnectAsync(url, username, password);



        ////////////////////////////////////////////////////////////////////
        // FILE CABINET
        ////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Retrieves all file cabinets asynchronously.
        /// </summary>
        /// <returns>A <see cref="ServiceResult{List{FileCabinet}}"/> or an error message.</returns>
        public Task<ServiceResult<List<FileCabinet>>> GetFileCabinetsAsync() => _fileCabinetService.GetFileCabinetsAsync();

        /// <summary>
        /// Retrieves a specific file cabinet by ID.
        /// </summary>
        /// <param name="fileCabinetId">The file cabinet ID.</param>
        /// <returns>A <see cref="ServiceResult{FileCabinet}"/> or an error message.</returns>
        public ServiceResult<FileCabinet> GetFileCabinetById(string fileCabinetId) => _fileCabinetService.GetFileCabinetById(fileCabinetId);



        ////////////////////////////////////////////////////////////////////
        // DOCUMENT
        ////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Retrieves documents from a file cabinet asynchronously.
        /// </summary>
        /// <param name="fileCabinetId">The file cabinet ID.</param>
        /// <param name="count">Optional maximum number of documents to retrieve (default 10000).</param>
        /// <returns>A <see cref="ServiceResult{List{Document}}"/> or an error message.</returns>
        public Task<ServiceResult<List<Document>>> GetDocumentsAsync(string fileCabinetId, int? count = 10000) => _documentService.GetDocumentsByFileCabinetIdAsync(fileCabinetId, count);

        /// <summary>
        /// Retrieves a document by its ID asynchronously.
        /// </summary>
        /// <param name="fileCabinetId">The file cabinet ID.</param>
        /// <param name="docId">The document ID.</param>
        /// <returns>A <see cref="ServiceResult{Document}"/> containing the document or an error message.</returns>
        public async Task<ServiceResult<Document>> GetDocumentByIdAsync(string fileCabinetId, string docId) => await _documentService.GetDocumentByIdAsync(fileCabinetId, docId);

        /// <summary>
        /// Updates a single document's fields asynchronously.
        /// </summary>
        /// <param name="doc">The document to update.</param>
        /// <param name="fields">Fields to update.</param>
        /// <param name="forceUpdate">Force update even if fields are unchanged.</param>
        /// <returns>A <see cref="ServiceResult{bool}"/> indicating success or failure.</returns>
        public Task<ServiceResult<bool>> UpdateDocumentFieldsAsync(Document doc, Dictionary<string, string> fields, bool forceUpdate) => _documentService.UpdateDocFieldsAsync(doc, fields, forceUpdate);

        /// <summary>
        /// Updates multiple documents in batch asynchronously.
        /// </summary>
        /// <param name="fileCabinetId">The file cabinet ID.</param>
        /// <param name="documentIds">Document IDs to update.</param>
        /// <param name="fields">Fields to update.</param>
        /// <param name="forceUpdate">Force update even if fields are unchanged.</param>
        /// <returns>A <see cref="ServiceResult{bool}"/> indicating success or failure.</returns>
        public Task<ServiceResult<bool>> BatchUpdateDocumentFieldsAsync(string fileCabinetId, List<int> documentIds, Dictionary<string, string> fields, bool forceUpdate) => _documentService.BatchUpdateDocFieldsAsync(fileCabinetId, documentIds, fields, forceUpdate);

        /// <summary>
        /// Appends keyword index fields to multiple documents asynchronously.
        /// </summary>
        /// <param name="fileCabinetId">The file cabinet ID.</param>
        /// <param name="documentIds">Document IDs to update.</param>
        /// <param name="keywordsFieldName">Keyword index field name.</param>
        /// <param name="keywordValues">Keyword values to append.</param>
        /// <param name="storeDialogId">The store dialog ID.</param>
        /// <param name="forceUpdate">Force update even if fields are unchanged.</param>
        /// <returns>A <see cref="ServiceResult{bool}"/> indicating success or failure.</returns>
        public Task<ServiceResult<bool>> BatchAppendKeywordIndexFieldsAsync(string fileCabinetId, List<int> documentIds, string keywordsFieldName, List<string> keywordValues, string storeDialogId, bool forceUpdate) => _documentService.BatchUpdateKeywordIndexFieldsAsync(fileCabinetId, documentIds, keywordsFieldName, keywordValues, storeDialogId, forceUpdate);

        /// <summary>
        /// Retrieves the index fields of a document asynchronously.
        /// </summary>
        /// <param name="document">The document to retrieve fields from.</param>
        /// <returns>A <see cref="ServiceResult{List{DocumentIndexField}}"/> containing document fields or an error message.</returns>
        public async Task<ServiceResult<List<DocumentIndexField>>> GetDocFieldsAsync(Document document) => await _documentService.GetDocFieldsAsync(document);

        /// <summary>
        /// Retrieves the index fields of a document by file cabinet and document ID asynchronously.
        /// </summary>
        /// <param name="fileCabinetId">The file cabinet ID.</param>
        /// <param name="docId">The document ID.</param>
        /// <returns>A <see cref="ServiceResult{List{DocumentIndexField}}"/> containing document fields or an error message.</returns>
        public async Task<ServiceResult<List<DocumentIndexField>>> GetDocFieldsAsync(string fileCabinetId, string docId) => await _documentService.GetDocFieldsAsync(fileCabinetId, docId);



        ////////////////////////////////////////////////////////////////////
        // ORGANIZATION
        ////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Retrieves the current organization synchronously.
        /// </summary>
        /// <returns>A <see cref="ServiceResult{Organization}"/> containing the organization or an error message.</returns>
        public ServiceResult<Organization> GetOrganization() => _organizationService.GetOrganization();

        /// <summary>
        /// Retrieves all organizations asynchronously.
        /// </summary>
        /// <returns>A <see cref="ServiceResult{List{Organization}}"/> containing a list of organizations or an error message.</returns>
        public Task<ServiceResult<List<Organization>>> GetOrganizationsAsync() => _organizationService.GetOrganizationsAsync();
    }
}