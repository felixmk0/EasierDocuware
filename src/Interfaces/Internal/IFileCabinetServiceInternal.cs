using DocuWare.Platform.ServerClient;
using EasierDocuware.Models.Global;

namespace EasierDocuware.Interfaces.Internal
{
    public interface IFileCabinetServiceInternal
    {
        ServiceResult<FileCabinet> GetFileCabinetById(string fileCabinetId);
        Task<ServiceResult<List<FileCabinet>>> GetFileCabinetsAsync();
    }
}