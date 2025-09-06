using EasierDocuware.Interfaces;
using EasierDocuware.Interfaces.Public;
using EasierDocuware.Models;


namespace EasierDocuware.Services.Public
{
    public class AuthService : IAuthService
    {
        private readonly IAuthServiceInternal _internalAuthService;

        public AuthService(IAuthServiceInternal internalAuthService)
        {
            _internalAuthService = internalAuthService;
        }

        
        public Task<ServiceResult<bool>> ConnectAsync(string url, string username, string password)
        {
            return _internalAuthService.ConnectAsync(url, username, password);
        }
    }
}