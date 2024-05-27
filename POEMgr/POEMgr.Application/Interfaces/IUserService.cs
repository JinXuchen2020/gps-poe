using POEMgr.Application.TransferModels;

namespace POEMgr.Application.Interfaces
{
    public interface IUserService
    {
        Task<ApiResult> User_getCurrentUser(User_getCurrentUser_req p);
        Task<ApiResult> User_add(User_add_req p);
        Task<ApiResult> GetUsers(GetUsersRequest p);

        Task<ApiResult> UpdateUser(string id, UpdateUserRequest p);

        Task<ApiResult> DeleteUsers(List<string> ids);

        Task<ApiResult> GetRoles();
    }
}