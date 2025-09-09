using DocuWare.Platform.ServerClient;
using EasierDocuware.Interfaces.Internal;
using EasierDocuware.Models.Global;

namespace EasierDocuware.Interfaces.Public
{
    public interface IOrganizationService : IOrganizationServiceInternal
    {
        ServiceResult<Organization> GetOrganization();
        Task<ServiceResult<List<Organization>>> GetOrganizationsAsync();
    }
}