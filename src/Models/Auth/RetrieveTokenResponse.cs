namespace EasierDocuware.Models.Auth
{
    public class RetrieveTokenResponse
    {
        public RetrieveTokenResponse(string accessToken, string refreshToken, string error
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            Error = error;
        }

        public string AccessToken { get; }
        public string RefreshToken { get; }
        public string Error { get; }
    }
}