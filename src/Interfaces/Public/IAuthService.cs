using EasierDocuware.Models;

namespace EasierDocuware.Interfaces.Public
{
    public interface IAuthService
    {
        Task<ServiceResult<bool>> ConnectAsync(string url, string username, string password);
    }
}