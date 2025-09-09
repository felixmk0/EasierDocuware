using DocuWare.Platform.ServerClient;
using EasierDocuware.Interfaces.Internal;
using EasierDocuware.Interfaces.Public;
using EasierDocuware.Models.Global;


namespace EasierDocuware.Services.Public
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IOrganizationServiceInternal _internal;

        public OrganizationService(IOrganizationServiceInternal internalService)
        {
            _internal = internalService;
        }


        public ServiceResult<Organization> GetOrganization() => _internal.GetOrganization();
        public Task<ServiceResult<List<Organization>>> GetOrganizationsAsync() => _internal.GetOrganizationsAsync();
    }
}
