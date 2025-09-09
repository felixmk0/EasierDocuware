using DocuWare.Platform.ServerClient;
using EasierDocuware.Interfaces.Internal;
using EasierDocuware.Interfaces.Public;
using EasierDocuware.Models.Global;


namespace EasierDocuware.Services.Public
{
    public class FileCabinetService : IFileCabinetService
    {
        private readonly IFileCabinetServiceInternal _internal;

        public FileCabinetService(IFileCabinetServiceInternal internalService)
        {
            _internal = internalService;
        }

        public ServiceResult<FileCabinet> GetFileCabinetById(string fileCabinetId) => _internal.GetFileCabinetById(fileCabinetId);

        public Task<ServiceResult<List<FileCabinet>>> GetFileCabinetsAsync() => _internal.GetFileCabinetsAsync();
    }
}