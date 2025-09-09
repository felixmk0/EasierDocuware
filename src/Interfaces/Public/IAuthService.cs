using EasierDocuware.Models.Auth;
using EasierDocuware.Models.Global;
using System.Threading.Tasks;

namespace EasierDocuware.Interfaces.Public
{
    public interface IAuthService
    {
        Task<ServiceResult<bool>> ConnectAsync(string url, string username, string password);
        Task<ServiceResult<bool>> ConnectAppRegistrationAsync(AppRegistration appRegistration);
        Task<ServiceResult<bool>> DisconnectAsync();
    }
}