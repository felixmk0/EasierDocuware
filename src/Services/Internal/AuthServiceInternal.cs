using DocuWare.Platform.ServerClient;
using EasierDocuware.Interfaces;
using EasierDocuware.Models;


namespace EasierDocuware.Services.Internal
{
    public class AuthServiceInternal : IAuthServiceInternal
    {
        private ServiceConnection _serviceConnection;

        public AuthServiceInternal(ServiceConnection serviceConnection)
        {
            _serviceConnection = serviceConnection;
        }


        public async Task<ServiceResult<bool>> ConnectAsync(string url, string username, string password)
        {
            try
            {
                var connection = await ServiceConnection.CreateAsync(new Uri(url), username, password);
                if (connection == null) return ServiceResult<bool>.Fail("Connection failed!");

                _serviceConnection = connection;
                return ServiceResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<bool>> DisconnectAsync()
        {
            try
            {
                if (_serviceConnection == null) return ServiceResult<bool>.Fail("No active connection to disconnect.");

                await _serviceConnection.DisconnectAsync();
                _serviceConnection = null!;

                return ServiceResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Fail(ex.Message);
            }
        }

        ServiceResult<ServiceConnection> IAuthServiceInternal.IsConnectedAsync()
        {
            try
            {
                if (_serviceConnection == null) return ServiceResult<ServiceConnection>.Fail("Not connected!");
                return ServiceResult<ServiceConnection>.Ok(_serviceConnection);
            }
            catch (Exception ex)
            {
                return ServiceResult<ServiceConnection>.Fail(ex.Message);
            }
        }
    }
}