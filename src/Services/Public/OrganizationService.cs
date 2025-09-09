using DocuWare.Platform.ServerClient;
using EasierDocuware.Interfaces.Public;
using EasierDocuware.Models.Global;


namespace EasierDocuware.Services.Public
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IOrganizationService _organizationService;

        public OrganizationService(IOrganizationService organizationService)
        {
            _organizationService = organizationService;
        }


        public ServiceResult<Organization> GetOrganization() => _organizationService.GetOrganization();
        public Task<ServiceResult<List<Organization>>> GetOrganizationsAsync() => _organizationService.GetOrganizationsAsync();
    }
}
