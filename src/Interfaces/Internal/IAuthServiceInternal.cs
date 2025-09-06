using EasierDocuware.Interfaces.Public;
using EasierDocuware.Models;

namespace EasierDocuware.Interfaces
{
    public interface IAuthServiceInternal : IAuthService
    {
        ServiceResult<DocuWare.Platform.ServerClient.ServiceConnection> IsConnectedAsync();
    }
}