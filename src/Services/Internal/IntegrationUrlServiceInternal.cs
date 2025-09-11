using DocuWare.WebIntegration;
using EasierDocuware.Models.Global;
using System.Globalization;


namespace EasierDocuware.Services.Internal
{
    public class IntegrationUrlServiceInternal
    {
        private DWIntegrationInfo dWIntegrationInfo;

        public IntegrationUrlServiceInternal()
        {

        }


        public void Init(string serverAddress, string organizationGuid, string serverUrl)
        {
            dWIntegrationInfo = new DWIntegrationInfo(serverUrl, organizationGuid, false);
        }

        public void EnsureInitialized()
        {
            if (dWIntegrationInfo == null) throw new InvalidOperationException("IntegrationUrlServiceInternal is not initialized. Call Init() method first.");
        }


        public async Task<ServiceResult<Uri>> GenerateDocumentViewerAsync(string fileCabinetId, int docId)
        {
            try
            {
                EnsureInitialized();
                var integrationType = IntegrationType.Viewer;

                var dwParam = new DWIntegrationUrlParameters(integrationType)
                {
                    FileCabinetGuid = new Guid(fileCabinetId),
                    DocId = docId.ToString(),
                    Culture = new CultureInfo("en-En")
                };

                var dwUrl = new DWIntegrationUrl(dWIntegrationInfo, dwParam);
                return ServiceResult<Uri>.Ok(new Uri(dwUrl.Url));
            }
            catch (Exception ex)
            {
                return ServiceResult<Uri>.Fail(ex.Message);
            }
        }
    }
}