using DocuWare.Platform.ServerClient;
using EasierDocuware.Interfaces.Internal;
using EasierDocuware.Models;


namespace EasierDocuware.Services.Internal
{
    public class UserServiceInternal
    {
        private readonly IOrganizationServiceInternal _organizationService;

        public UserServiceInternal(IOrganizationServiceInternal organizationService)
        {
            _organizationService = organizationService;
        }


        public async Task<ServiceResult<bool>> CreateUserAsync(string organizationId, string userName, string userEmail, string password)
        {
            try
            {
                var orgResult = _organizationService.GetOrganization();
                if (!orgResult.Success) return ServiceResult<bool>.Fail(orgResult.Message!);
                var org = orgResult.Data!;

                NewUser createUser = new NewUser
                {
                    DbName = userName,
                    Name = userName,
                    Email = userEmail,
                    NetworkId = "",
                    Password = password
                };

                var result = await org.PostToUserInfoRelationForUserAsync(createUser);
                if (!result.IsSuccessStatusCode) throw new Exception($"Create user failed with status code {result.StatusCode}: {result.Exception.Message}");

                return ServiceResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<List<User>>> GetUsersAsync(string organizationId)
        {
            try
            {
                var orgResult = _organizationService.GetOrganization();
                if (!orgResult.Success) return ServiceResult<List<User>>.Fail(orgResult.Message!);
                var org = orgResult.Data!;

                var reponse = await org.GetUsersFromUsersRelationAsync();
                if (!reponse.IsSuccessStatusCode) throw new Exception($"Get users failed with status code {reponse.StatusCode}: {reponse.Exception.Message}");
                var users = reponse.Content.User;

                return ServiceResult<List<User>>.Ok(users);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<User>>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<User>> GetUserByIdAsync(string organizationId, string userId)
        {
            try
            {
                var orgResult = _organizationService.GetOrganization();
                if (!orgResult.Success) return ServiceResult<User>.Fail(orgResult.Message!);
                var org = orgResult.Data!;

                var reponse = await org.GetUsersFromUsersRelationAsync();
                if (!reponse.IsSuccessStatusCode) throw new Exception($"Get users failed with status code {reponse.StatusCode}: {reponse.Exception.Message}");

                var user = reponse.Content.User.FirstOrDefault(u => u.Id == userId);
                if (user == null) return ServiceResult<User>.Fail("User not found!");

                return ServiceResult<User>.Ok(user);
            }
            catch (Exception ex)
            {
                return ServiceResult<User>.Fail(ex.Message);
            }
        }


        // FALTA ACABAR DEL TOT 
        public async Task<ServiceResult<bool>> AssignUserToRoleAsync(User user, Role role, AssignmentOperationType operationType)
        {
            try
            {
                //Add user to the role
                await user.PutToGroupsRelationForStringAsync(new AssignmentOperation { OperationType = AssignmentOperationType.Add, Ids = new List<string>() { role.Id } });

                //Remove user form the role
                await user.PutToGroupsRelationForStringAsync(new AssignmentOperation { OperationType = AssignmentOperationType.Remove, Ids = new List<string>() { role.Id } });

                return ServiceResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Fail(ex.Message);
            }
        }


        // FALTA ACABAR DEL TOT 
        public async Task<ServiceResult<bool>> AssignUserToGroupAsync(User user, Group group, AssignmentOperationType operationType)
        {
            try
            {
                if (operationType.Equals(AssignmentOperationType.Add))
                {
                    await user.PutToGroupsRelationForStringAsync(new AssignmentOperation { OperationType = AssignmentOperationType.Add, Ids = new List<string>() { group.Id } });
                }
                else if (operationType.Equals(AssignmentOperationType.Remove))
                {
                    await user.PutToGroupsRelationForStringAsync(new AssignmentOperation { OperationType = AssignmentOperationType.Remove, Ids = new List<string>() { group.Id } });
                }
                else
                {
                    return ServiceResult<bool>.Fail("Invalid operation type!");
                }

                return ServiceResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Fail(ex.Message);
            }
        }
    }
}