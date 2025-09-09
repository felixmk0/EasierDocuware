using EasierDocuware.Interfaces.Public;
using EasierDocuware.Models.Global;

namespace EasierDocuware.Interfaces
{
    public interface IAuthServiceInternal : IAuthService
    {
        ServiceResult<DocuWare.Platform.ServerClient.ServiceConnection> IsConnectedAsync();
    }
}