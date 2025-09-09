using DocuWare.Platform.ServerClient;
using EasierDocuware.Interfaces;
using EasierDocuware.Interfaces.Internal;
using EasierDocuware.Models.Global;


namespace EasierDocuware.Services.Internal
{
    public class OrganizationServiceInternal : IOrganizationServiceInternal
    {
        private readonly IAuthServiceInternal _authService;

        public OrganizationServiceInternal(IAuthServiceInternal authService)
        {
            _authService = authService;
        }


        public ServiceResult<Organization> GetOrganization()
        {
            try
            {
                var authResult = _authService.IsConnectedAsync();
                if (!authResult.Success) return ServiceResult<Organization>.Fail(authResult.Message!);

                var _serviceConnection = authResult.Data!;
                var org = _serviceConnection.Organizations[0];

                if (org == null) return ServiceResult<Organization>.Fail("No organization found!");
                return ServiceResult<Organization>.Ok(org);
            }
            catch (Exception ex)
            {
                return ServiceResult<Organization>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<List<Organization>>> GetOrganizationsAsync()
        {
            try
            {
                var authResult = _authService.IsConnectedAsync();
                if (!authResult.Success) return ServiceResult<List<Organization>>.Fail(authResult.Message!);

                var _serviceConnection = authResult.Data!;

                var orgs = await _serviceConnection.GetOrganizationsAsync();
                if (orgs == null || !orgs.Any()) return ServiceResult<List<Organization>>.Fail("No organization found!");

                return ServiceResult<List<Organization>>.Ok(orgs.ToList());
            }
            catch (Exception ex)
            {
                return ServiceResult<List<Organization>>.Fail(ex.Message);
            }
        }
    }
}