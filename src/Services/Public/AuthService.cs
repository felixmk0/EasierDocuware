using EasierDocuware.Interfaces;
using EasierDocuware.Interfaces.Public;
using EasierDocuware.Models;


namespace EasierDocuware.Services.Public
{
    /// <summary>
    /// Service for authentication with DocuWare.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IAuthServiceInternal _internalAuthService;

        public AuthService(IAuthServiceInternal internalAuthService)
        {
            _internalAuthService = internalAuthService;
        }

        /// <summary>
        /// Connects to the DocuWare server using the provided credentials.
        /// </summary>
        /// <param name="url">The URL of the DocuWare server.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>A <see cref="ServiceResult{T}"/> indicating success or failure.</returns>
        public Task<ServiceResult<bool>> ConnectAsync(string url, string username, string password)
        {
            return _internalAuthService.ConnectAsync(url, username, password);
        }
    }
}