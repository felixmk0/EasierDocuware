using DocuWare.Platform.ServerClient;
using EasierDocuware.Interfaces.Internal;
using EasierDocuware.Models.Global;

namespace EasierDocuware.Interfaces.Public
{
    public interface IFileCabinetService : IFileCabinetServiceInternal
    {
        ServiceResult<FileCabinet> GetFileCabinetById(string fileCabinetId);
        Task<ServiceResult<List<FileCabinet>>> GetFileCabinetsAsync();
    }
}