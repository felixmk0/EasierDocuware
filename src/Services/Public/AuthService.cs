using EasierDocuware.Interfaces;
using EasierDocuware.Interfaces.Public;
using EasierDocuware.Models.Auth;
using EasierDocuware.Models.Global;


namespace EasierDocuware.Services.Public
{
    public class AuthService : IAuthService
    {
        private readonly IAuthService _authService;

        public AuthService(IAuthService authService)
        {
            _authService = authService;
        }


        public Task<ServiceResult<bool>> ConnectAsync(string url, string username, string password) => _authService.ConnectAsync(url, username, password);
        public Task<ServiceResult<bool>> ConnectAppRegistrationAsync(AppRegistration appRegistration) => _authService.ConnectAppRegistrationAsync(appRegistration);
        public Task<ServiceResult<bool>> DisconnectAsync() => _authService.DisconnectAsync();
    }
}