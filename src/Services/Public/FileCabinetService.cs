using DocuWare.Platform.ServerClient;
using EasierDocuware.Interfaces.Internal;
using EasierDocuware.Interfaces.Public;
using EasierDocuware.Models;


namespace EasierDocuware.Services.Internal
{
    public class FileCabinetService : IFileCabinetService
    {
        private readonly IFileCabinetServiceInternal _fileCabinetServiceInternal;

        public FileCabinetService(IFileCabinetServiceInternal fileCabinetServiceInternal)
        {
            _fileCabinetServiceInternal = fileCabinetServiceInternal;
        }


        public ServiceResult<FileCabinet> GetFileCabinetById(string fileCabinetId)
        {
            return _fileCabinetServiceInternal.GetFileCabinetById(fileCabinetId);
        }

        public Task<ServiceResult<List<FileCabinet>>> GetFileCabinetsAsync()
        {
            return _fileCabinetServiceInternal.GetFileCabinetsAsync();
        }
    }
}