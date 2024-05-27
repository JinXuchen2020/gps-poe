using AutoMapper;
using LogService;
using POEMgr.Application.TransferModels;
using POEMgr.Domain.IRepositories;
using POEMgr.Domain.Models;
using POEMgr.Repository.DBContext;

namespace POEMgr.Application.Services
{
    internal class SampleService
    {
        private readonly POEContext _poeContext;

        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public SampleService(IMapper mapper, POEContext poeContext)
        {
            _poeContext = poeContext;
        }

        public SampleService(IMapper mapper,
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            POEContext poeContext)
        {
            this._userRepository = userRepository;
            this._roleRepository = roleRepository;
        }

        #region Example
        //public async Task<UserResponse> AddUserAsync(AddUserRequest model)
        //{
        //    User user = this._mapper.Map<User>(model);

        //    await this._userRepository.InsertAsync(user);

        //    return new UserResponse()
        //    {
        //        Id = user.Id.ToString(),
        //        UserName = user.Name,
        //        Alias = user.Alias,
        //        Mail = user.Email
        //    };
        //}

        //public async Task<UserResponse> GetUserById(Guid id)
        //{
        //    var user = await this._userRepository.GetByIdAsync(id);
        //    var response = this._mapper.Map<UserResponse>(user);

        //    return response;
        //}

        //public async Task<string> GrantPermission(Guid userId, Guid roleId)
        //{
        //    User user = await this._userRepository.GetByIdAsync(userId);
        //    Role role = await this._roleRepository.GetByIdAsync(roleId);

        //    user.AddRole(role);
        //    await this._userRepository.UpdateAsync(user);

        //    return user.Name;
        //}

        #endregion
    }
}
