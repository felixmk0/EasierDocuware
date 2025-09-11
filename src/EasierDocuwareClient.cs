using DocuWare.Platform.ServerClient;
using EasierDocuware.Interfaces.Internal;
using EasierDocuware.Interfaces.Public;
using EasierDocuware.Models.Auth;
using EasierDocuware.Models.Documents;
using EasierDocuware.Models.Global;
using EasierDocuware.Services.Internal;
using EasierDocuware.Services.Public;


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

        public EasierDocuwareClient()
        {
            var authServiceInternal = new AuthServiceInternal(serviceConnection: null!);
            var fileCabinetServiceInternal = new FileCabinetServiceInternal(authServiceInternal);
            var documentServiceInternal = new DocumentServiceInternal(authServiceInternal, fileCabinetServiceInternal);
            var organizationServiceInternal = new OrganizationServiceInternal(authServiceInternal);

            _authService = new AuthService(authServiceInternal);
            _fileCabinetService = new FileCabinetService(fileCabinetServiceInternal);
            _documentService = new DocumentService(documentServiceInternal);
            _organizationService = new OrganizationService(organizationServiceInternal);
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

        /// <summary>
        /// Connects to the DocuWare server using an app registration.
        /// </summary>
        /// <param name="appRegistration">The app registration object containing the credentials and configuration.</param>
        /// <returns>A <see cref="ServiceResult{bool}"/> indicating success or failure.</returns>
        public Task<ServiceResult<bool>> ConnectAppRegistrationAsync(AppRegistration appRegistration) => _authService.ConnectAppRegistrationAsync(appRegistration);

        /// <summary>
        /// Disconnects from the DocuWare server.
        /// </summary>
        /// <returns>A <see cref="ServiceResult{bool}"/> indicating success or failure.</returns>
        public Task<ServiceResult<bool>> DisconnectAsync() => _authService.DisconnectAsync();



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
        public async Task<ServiceResult<Document>> GetDocumentByIdAsync(string fileCabinetId, int docId) => await _documentService.GetDocumentByIdAsync(fileCabinetId, docId);

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
        public async Task<ServiceResult<List<DocumentIndexField>>> GetDocFieldsAsync(string fileCabinetId, int docId) => await _documentService.GetDocFieldsAsync(fileCabinetId, docId);

        /// <summary>
        /// Downloads a document asynchronously.
        /// </summary>
        /// <param name="fileCabinetId">The file cabinet ID.</param>
        /// <param name="docId">The document ID.</param>
        /// <returns>A <see cref="ServiceResult{DocumentFileDownload}"/> containing the file stream with other details or an error message.</returns>
        public Task<ServiceResult<DocumentFileDownloadResult>> DownloadDocumentAsync(string fileCabinetId, int docId) => _documentService.DownloadDocumentAsync(fileCabinetId, docId);

        /// <summary>
        /// Exports a document as DWX asynchronously.
        /// </summary>
        /// <param name="fileCabinetId">The file cabinet ID.</param>
        /// <param name="docId">The document ID.</param>
        /// <param name="exportSettings">Settings for the export.</param>
        /// <returns>A <see cref="ServiceResult{FileStream}"/> containing the exported DWX stream or an error message.</returns>
        public Task<ServiceResult<FileStream>> ExportDocumentAsDwxAsync(string fileCabinetId, int docId, ExportSettings exportSettings) => _documentService.ExportDocumentAsDwxAsync(fileCabinetId, docId, exportSettings);

        /// <summary>
        /// Imports a DWX document asynchronously from a stream.
        /// </summary>
        /// <param name="fileCabinetId">The file cabinet ID.</param>
        /// <param name="dwxStream">The DWX file stream.</param>
        /// <param name="fileName">The file name for the temporary import.</param>
        /// <returns>A <see cref="ServiceResult{List{int}}"/> containing the IDs of imported documents or an error message.</returns>
        public Task<ServiceResult<List<int>>> ImportDwxDocAsync(string fileCabinetId, Stream dwxStream, string fileName) => _documentService.ImportDwxDocAsync(fileCabinetId, dwxStream, fileName);

        /// <summary>
        /// Finds duplicate documents by matching field values asynchronously.
        /// </summary>
        /// <param name="fileCabinetId">The file cabinet ID.</param>
        /// <param name="fields">Fields and values to check for duplicates.</param>
        /// <returns>A <see cref="ServiceResult{List{Document}}"/> containing duplicate documents or an error message.</returns>
        public Task<ServiceResult<List<Document>>> GetDuplicateDocsAsync(string fileCabinetId, Dictionary<string, object> fields) => _documentService.GetDuplicateDocsAsync(fileCabinetId, fields);

        /// <summary>
        /// Finds IDs of duplicate documents by matching field values asynchronously.
        /// </summary>
        /// <param name="fileCabinetId">The file cabinet ID.</param>
        /// <param name="fields">Fields and values to check for duplicates.</param>
        /// <returns>A <see cref="ServiceResult{List{int}}"/> containing IDs of duplicate documents or an error message.</returns>
        public Task<ServiceResult<List<int>>> GetDuplicateDocsIdsAsync(string fileCabinetId, Dictionary<string, object> fields) => _documentService.GetDuplicateDocsIdsAsync(fileCabinetId, fields);



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