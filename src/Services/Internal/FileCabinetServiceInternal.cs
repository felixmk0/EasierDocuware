using DocuWare.Platform.ServerClient;
using EasierDocuware.Interfaces;
using EasierDocuware.Interfaces.Internal;
using EasierDocuware.Models;


namespace EasierDocuware.Services.Internal
{
    public class FileCabinetServiceInternal : IFileCabinetServiceInternal
    {
        private readonly IAuthServiceInternal _authService;

        public FileCabinetServiceInternal(IAuthServiceInternal authService)
        {
            _authService = authService;
        }


        public ServiceResult<FileCabinet> GetFileCabinetById(string fileCabinetId)
        {
            try
            {
                var authResult = _authService.IsConnectedAsync();
                if (!authResult.Success) return ServiceResult<FileCabinet>.Fail(authResult.Message!);

                var _serviceConnection = authResult.Data!;
                if (string.IsNullOrEmpty(fileCabinetId)) return ServiceResult<FileCabinet>.Fail("File cabinet id cannot be null!");

                var fileCabinet = _serviceConnection.GetFileCabinet(fileCabinetId);
                if (fileCabinet == null) return ServiceResult<FileCabinet>.Fail("File cabinet not found!");

                return ServiceResult<FileCabinet>.Ok(fileCabinet);
            }
            catch (Exception ex)
            {
                return ServiceResult<FileCabinet>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<List<FileCabinet>>> GetFileCabinetsAsync()
        {
            try
            {
                var authResult = _authService.IsConnectedAsync();
                if (!authResult.Success) return ServiceResult<List<FileCabinet>>.Fail(authResult.Message!);

                var _serviceConnection = authResult.Data!;
                var org = _serviceConnection.Organizations[0];
                if (org == null) return ServiceResult<List<FileCabinet>>.Fail("No organization found!");

                var response = await org.GetFileCabinetsFromFilecabinetsRelationAsync();
                if (!response.IsSuccessStatusCode) return ServiceResult<List<FileCabinet>>.Fail($"Get file cabinets failed with status code {response.StatusCode}: {response.Exception.Message}");

                return ServiceResult<List<FileCabinet>>.Ok(response.Content.FileCabinet);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<FileCabinet>>.Fail(ex.Message);
            }
        }
    }
}