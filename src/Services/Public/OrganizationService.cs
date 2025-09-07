using DocuWare.Platform.ServerClient;
using EasierDocuware.Interfaces.Internal;
using EasierDocuware.Interfaces.Public;
using EasierDocuware.Models;


namespace EasierDocuware.Services.Public
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IOrganizationServiceInternal _organizationServiceInternal;

        public OrganizationService(IOrganizationServiceInternal organizationServiceInternal)
        {
            _organizationServiceInternal = organizationServiceInternal;
        }


        public ServiceResult<Organization> GetOrganization() => _organizationServiceInternal.GetOrganization();
        public Task<ServiceResult<List<Organization>>> GetOrganizationsAsync() => _organizationServiceInternal.GetOrganizationsAsync();
    }
}
