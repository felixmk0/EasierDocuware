namespace EasierDocuware.Models.Auth
{
    public class AppRegistration
    {
        public required string ClientId { get; set; }
        public required string ClientSecret { get; set; }
        public required string Scope { get; set; }
        public required string RedirectUri { get; set; }
        public required string PlatformUrl { get; set; }
        public required string OrganizationGuid { get; set; }
    }
}