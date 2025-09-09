using DocuWare.Platform.ServerClient;
using Duende.IdentityModel.OidcClient;
using EasierDocuware.Interfaces;
using EasierDocuware.Models.Auth;
using EasierDocuware.Models.Global;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Web;


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

        // TOKEN REFRESHING NOT IMPLEMENTED YET
        public async Task<ServiceResult<bool>> ConnectAppRegistrationAsync(AppRegistration appRegistration)
        {
            try
            {
                var response = await AuthorizationCodeFlow(appRegistration);
                // await Console.Out.WriteLineAsync(response.RefreshToken);

                var connect = await ServiceConnection.CreateWithJwtAsync(new Uri(appRegistration.PlatformUrl), response.AccessToken, DWProductTypes.PlatformService);
                if (connect == null) return ServiceResult<bool>.Fail("Connection failed!");

                _serviceConnection = connect;

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



        private static async Task<RetrieveTokenResponse> AuthorizationCodeFlow(AppRegistration appRegistration)
        {
            var idsInfo = await ServiceConnection.GetIdentityServiceInfoAsync(new Uri(appRegistration.PlatformUrl), new IdentityServiceInfoConnectionData());
            var oidcClient = new OidcClient(new OidcClientOptions()
            {
                Authority = idsInfo.IdentityServiceUrl,
                ClientId = appRegistration.ClientId,
                ClientSecret = appRegistration.ClientSecret,
                Scope = appRegistration.Scope,
                RedirectUri = appRegistration.RedirectUri,
                FilterClaims = true,
                LoadProfile = false,
                Policy = new Policy { Discovery = new Duende.IdentityModel.Client.DiscoveryPolicy { RequireHttps = false } },
            });

            var response = await RetrieveAccessTokenAsync(oidcClient, appRegistration.OrganizationGuid).ConfigureAwait(false);
            return response;
        }

        private static async Task<RetrieveTokenResponse> RetrieveAccessTokenAsync(OidcClient client, string organizationGuid)
        {
            var state = await client.PrepareLoginAsync();

            using var http = new HttpListener();
            http.Prefixes.Add(state.RedirectUri);
            http.Start();

            var browserProcess = Process.Start(new ProcessStartInfo("cmd", $"/c start {state.StartUrl.Replace("&", "^&")}") { CreateNoWindow = true });

            var context = await http.GetContextAsync();
            var requestQueryString = context.Request.QueryString;

            await SendSuccessMessageResponseAsync(context.Response, client.Options.Authority);
            browserProcess?.Close();

            var query = HttpUtility.ParseQueryString(string.Empty);
            query[QueryStringConstants.Code] = requestQueryString[QueryStringConstants.Code];
            query[QueryStringConstants.Scope] = requestQueryString[QueryStringConstants.Scope];
            query[QueryStringConstants.State] = requestQueryString[QueryStringConstants.State];
            query[QueryStringConstants.SessionState] = requestQueryString[QueryStringConstants.SessionState];

            var result = await client.ProcessResponseAsync(
                query.ToString(),
                state,
                new Duende.IdentityModel.Client.Parameters
                {
                { $"acr_values", $"tenant:{organizationGuid}" }
                }).ConfigureAwait(false);

            return new RetrieveTokenResponse(result.AccessToken, result.RefreshToken, result.IsError ? result.Error : string.Empty);
        }

        private static async Task SendSuccessMessageResponseAsync(HttpListenerResponse response, string serviceUrl)
        {
            var url = serviceUrl.EndsWith("/") ? serviceUrl : serviceUrl + "/";
            var responseString = "<html><head><meta http-equiv='Refresh' content='0; url = " + url + "Account/LoginSuccess' /></head><body></body></html>";
            var buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;

            var responseOutput = response.OutputStream;
            await responseOutput.WriteAsync(buffer, 0, buffer.Length);
            responseOutput.Close();
        }
    }
}