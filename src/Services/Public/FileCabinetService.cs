using DocuWare.Platform.ServerClient;
using EasierDocuware.Interfaces.Public;
using EasierDocuware.Models.Global;


namespace EasierDocuware.Services.Internal
{
    public class FileCabinetService : IFileCabinetService
    {
        private readonly IFileCabinetService _fileCabinetService;

        public FileCabinetService(IFileCabinetService fileCabinetService)
        {
            _fileCabinetService = fileCabinetService;
        }


        public ServiceResult<FileCabinet> GetFileCabinetById(string fileCabinetId) => _fileCabinetService.GetFileCabinetById(fileCabinetId);
        public Task<ServiceResult<List<FileCabinet>>> GetFileCabinetsAsync() => _fileCabinetService.GetFileCabinetsAsync();
    }
}