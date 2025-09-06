using DocuWare.Platform.ServerClient;
using EasierDocuware.Models;

namespace EasierDocuware.Interfaces.Internal
{
    public interface IOrganizationServiceInternal
    {
        ServiceResult<Organization> GetOrganization();
        Task<ServiceResult<List<Organization>>> GetOrganizationsAsync();
    }
}