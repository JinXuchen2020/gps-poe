using POEMgr.Application.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using POEMgr.Repository.DBContext;
using POEMgr.Application.TransferModels;
using POEMgr.Repository.DbModels;
using LogService;
using Authentication;
using System.Collections;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto;
using Microsoft.Data.SqlClient;
using POEMgr.Repository;

namespace POEMgr.Application.Services
{
    internal class UserService: BaseService, IUserService
    {

        public UserService(IMapper mapper, POEContext poeContext, IPoeLogService poeLogService, IClaimsAccessor claimsAccessor) 
            : base(mapper, poeContext, claimsAccessor, poeLogService)
        {
        }

        public async Task<ApiResult> User_getCurrentUser(User_getCurrentUser_req p)
        {
            ApiResult apiResult = new ApiResult();
            string newNumber = string.Empty;
            try
            {
                Sys_User sys_User = await _POEContext.Sys_User.Where(x => x.Mail.ToLower() == p.Email.ToLower()).FirstOrDefaultAsync();
                User_getCurrentUser_res res = new User_getCurrentUser_res();
                if (sys_User == null)
                {
                    List<Poe_Partner> checkPartner = await _POEContext.Poe_Partner.Where(x => x.Mail.ToLower().Contains(p.Email.ToLower())).ToListAsync();
                    if (checkPartner.Count > 0)
                    {
                        var role = await _POEContext.Sys_Role.Where(x => x.Id == Dictionaries.DefaultRoleId).FirstOrDefaultAsync();
                        Sys_User newSys_User = new Sys_User();
                        newSys_User.Id = p.UserId;
                        newSys_User.UserName = p.Name;
                        newSys_User.Mail = p.Email;
                        newSys_User.Alias = p.Name;

                        Sys_UserRole newSys_UserRole = new Sys_UserRole();
                        newSys_UserRole.Id = Guid.NewGuid().ToString();
                        newSys_UserRole.UserID = p.UserId;
                        newSys_UserRole.RoleID = role.Id;

                        _POEContext.Add(WriteEntityBase(newSys_User));
                        _POEContext.Add(WriteEntityBase(newSys_UserRole));
                        _POEContext.SaveChanges();

                        res.UserId = newSys_User.Id;
                        res.Name = newSys_User.UserName;
                        res.Email = newSys_User.Mail;
                        res.RoleId = role.Id;
                        res.RoleName = role.RoleName;
                    }
                }
                else
                {
                    if(sys_User.IsDisabled == "是")
                    {
                        throw new Exception($"Current user: {sys_User.Mail} is disabled!");
                    }
                    Sys_UserRole sys_UserRole = _POEContext.Sys_UserRole.Where(x => x.UserID == sys_User.Id && x.IsDeleted == false).First();
                    Sys_Role sys_Role = _POEContext.Sys_Role.Where(x => x.Id == sys_UserRole.RoleID).First();
                    res.UserId = sys_User.Id;
                    res.Name = sys_User.UserName;
                    res.Email = sys_User.Mail;
                    res.RoleId = sys_Role.Id;
                    res.RoleName = sys_Role.RoleName;
                }
                apiResult.Data = res;
                return apiResult;
            }
            catch (Exception ex)
            {
                CancelCurrentNumber("Sys_User", newNumber);
                apiResult.Code = -1;
                apiResult.Msg = ex.ToString();
            }
            return apiResult;
        }

        public async Task<ApiResult> GetUsers(GetUsersRequest p)
        {
            ApiResult apiResult = new ApiResult();
            try
            {
                var parameters = new List<SqlParameter>();
                var sqlString = Constants.GetUserPartnersSqlString;
                if (!string.IsNullOrEmpty(p.PartnerName))
                {
                    sqlString += " and pa.PartnerName like CONCAT('%', @partnerName,'%')";
                    parameters.Add(new SqlParameter("@partnerName", p.PartnerName));
                }

                if (!string.IsNullOrEmpty(p.PartnerEmail))
                {
                    sqlString += " and us.Mail like CONCAT('%', @partnerEmail,'%')";
                    parameters.Add(new SqlParameter("@partnerEmail", p.PartnerEmail));
                }

                List<GetUsersResponse> responses = _POEContext.Database.SqlQuery<GetUsersResponse>(sqlString, parameters.ToArray());
                var result = responses.GroupBy(c => c.Id).ToList().Select(c =>
                {
                    return new GetUsersResponse
                    {
                        Id = c.First().Id,
                        PartnerEmail = c.First().PartnerEmail,
                        PartnerName = string.Join(";", c.Select(c => c.PartnerName).Distinct()),
                        PartnerId = string.Join(";", c.Select(c => c.PartnerId).Distinct()),
                        IsDisabled = c.First().IsDisabled,
                        RoleName = c.First().RoleName,
                    };
                }).ToList();
                apiResult.Data = ToPaginationList(result, p.PageIndex, p.PageSize);
            }
            catch (Exception ex)
            {
                apiResult.Code = -1;
                apiResult.Msg = ex.ToString();
            }
            return await Task.FromResult(apiResult);
        }

        public async Task<ApiResult> UpdateUser(string id, UpdateUserRequest p)
        {
            ApiResult apiResult = new ApiResult();
            try
            {
                var role = _POEContext.Sys_Role.Where(x => x.RoleName == p.RoleName).FirstOrDefault();
                if (role != null)
                {
                    var user = _POEContext.Sys_User.Where(x => x.Mail == p.PartnerEmail).FirstOrDefault();
                    if (user != null) 
                    {
                        var userRole = _POEContext.Sys_UserRole.Where(x => x.IsDeleted == false && x.UserID == user.Id).FirstOrDefault();
                        if (userRole != null) 
                        {
                            userRole.IsDeleted = true;
                            _POEContext.Update(WriteEntityBase(userRole));
                        }

                        userRole = new Sys_UserRole
                        {
                            Id = Guid.NewGuid().ToString(),
                            UserID = user.Id,
                            RoleID = role.Id
                        };

                        _POEContext.Add(WriteEntityBase(userRole));

                        if(!string.IsNullOrEmpty(p.IsDisabled) && user.IsDisabled != p.IsDisabled)
                        {
                            user.IsDisabled = p.IsDisabled;
                            _POEContext.Update(WriteEntityBase(user));
                        }

                        await _POEContext.SaveChangesAsync();
                    }
                }

                WriteDbLog(DbLogType.Update, "UpdateUser", JsonConvert.SerializeObject(p));
            }
            catch (Exception ex)
            {
                apiResult.Code = -1;
                apiResult.Msg = ex.ToString();
            }
            return apiResult;
        }

        public async Task<ApiResult> DeleteUsers(List<string> ids)
        {
            ApiResult apiResult = new ApiResult();
            try
            {
                List<Poe_Partner> poe_Partner = _POEContext.Poe_Partner.Where(x => ids.Contains(x.Id)).ToList();
                poe_Partner.ForEach(x => x.IsDeleted = true);
                List<Sys_User> sys_User = _POEContext.Sys_User.Where(x => ids.Contains(x.Id)).ToList();
                sys_User.ForEach(x => x.IsDeleted = true);
                //List<Sys_UserRole> sys_UserRole = _poeContext.Sys_UserRole.Where(x => ids.Contains(x.UserID)).ToList();
                _POEContext.UpdateRange(poe_Partner);
                _POEContext.UpdateRange(sys_User);
                //_poeContext.RemoveRange(sys_UserRole);
                await _POEContext.SaveChangesAsync();
                apiResult.Data = ids;
                WriteDbLog(DbLogType.Delete, "DeleteUsers", string.Join(',',ids));
            }
            catch (Exception ex)
            {
                apiResult.Code = -1;
                apiResult.Msg = ex.ToString();
            }
            return apiResult;
        }

        public async Task<ApiResult> GetRoles()
        {
            ApiResult apiResult = new ApiResult();
            try
            {
                List<GetRolesResponse> sys_Role = await _POEContext.Sys_Role.Select(x => new GetRolesResponse
                {
                    RoleId = x.Id,
                    RoleName = x.RoleName
                }).OrderBy(x => x.RoleId).ToListAsync();

                apiResult.Data = sys_Role;
            }
            catch (Exception ex)
            {
                apiResult.Code = -1;
                apiResult.Msg = ex.ToString();
            }
            return apiResult;
        }

        public async Task<ApiResult> User_add(User_add_req p)
        {
            ApiResult apiResult = new ApiResult();
            string newNumber = string.Empty;
            try
            {
                bool checkUser = _POEContext.Sys_User.Any(x => x.Mail.ToLower() == p.Email);
                if (checkUser)
                {
                    apiResult.Code = 1;
                    apiResult.Msg = $"User mail already exists: {p.Email}";
                    return apiResult;
                }
                Sys_User sys_User = new Sys_User();
                sys_User.Id = GetCurrentNumber("Sys_User");
                newNumber = sys_User.Id;
                sys_User.UserName = p.Name;
                sys_User.Mail = p.Email;
                sys_User.Alias = p.Name;
                Poe_Partner poe_partner = new Poe_Partner();
                poe_partner.Id = sys_User.Id;
                poe_partner.PartnerName = p.Name;
                poe_partner.Mail = p.Email;
                Sys_UserRole sys_UserRole = new Sys_UserRole();
                sys_UserRole.Id = Guid.NewGuid().ToString();
                sys_UserRole.UserID= sys_User.Id;
                sys_UserRole.RoleID = string.IsNullOrEmpty(p.RoleId)?Dictionaries.DefaultRoleId: p.RoleId;
                await _POEContext.AddAsync(WriteEntityBase(poe_partner));
                await _POEContext.AddAsync(WriteEntityBase(sys_User));
                await _POEContext.AddAsync(WriteEntityBase(sys_UserRole));
                _POEContext.SaveChanges();
                User_add_res res = new User_add_res();
                res.Id= sys_User.Id;
                res.Name = sys_User.UserName;
                res.Alias=sys_User.Alias;
                res.RoleId = sys_UserRole.Id;
                res.Email = sys_User.Mail;
                apiResult.Data = res;
                WriteDbLog(DbLogType.Create, "User_add", JsonConvert.SerializeObject(p));
            }
            catch (Exception ex)
            {
                CancelCurrentNumber("Sys_User", newNumber);
                apiResult.Code = -1;
                apiResult.Msg = ex.ToString();
            }
            return apiResult;
        }

        //public async Task<ApiResult> DisableUser(string id)
        //{
        //    ApiResult apiResult = new ApiResult();
        //    try
        //    {
        //        Poe_Partner poe_Partner = await _poeContext.Poe_Partner.Where(x => x.Id == id).FirstOrDefaultAsync();
        //        if (poe_Partner != null)
        //        {
        //            poe_Partner.IsDisabled = "true";
        //            _poeContext.Update(poe_Partner);
        //            _poeContext.SaveChanges();
        //            Sys_UserRole Sys_UserRole= await _poeContext.Sys_UserRole.Where(x => x.Id == id).FirstOrDefaultAsync();
        //            apiResult.Data = new EnableOrDisableUserResponse
        //            {
        //                PartnerId = id,
        //                PartnerName = poe_Partner.PartnerName,
        //                IsDisabled= poe_Partner.IsDisabled,
        //            };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        apiResult.Code = -1;
        //        apiResult.Msg = ex.ToString();
        //    }
        //    return apiResult;
        //}

        //public async Task<ApiResult> EnableUser(string id)
        //{
        //    ApiResult apiResult = new ApiResult();
        //    try
        //    {
        //        Poe_Partner poe_Partner = await _poeContext.Poe_Partner.Where(x => x.Id == id).FirstOrDefaultAsync();
        //        if (poe_Partner != null)
        //        {
        //            poe_Partner.IsDisabled = "false";
        //            _poeContext.Update(poe_Partner);
        //            _poeContext.SaveChanges();
        //            Sys_UserRole Sys_UserRole = await _poeContext.Sys_UserRole.Where(x => x.Id == id).FirstOrDefaultAsync();
        //            apiResult.Data = new EnableOrDisableUserResponse
        //            {
        //                PartnerId = id,
        //                PartnerName = poe_Partner.PartnerName,
        //                IsDisabled = poe_Partner.IsDisabled,
        //            };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        apiResult.Code = -1;
        //        apiResult.Msg = ex.ToString();
        //    }
        //    return apiResult;
        //}

        //public async Task<ApiResult> GetUserRoles(string p)
        //{
        //    ApiResult apiResult = new ApiResult();
        //    try
        //    {
        //        List<Sys_UserRole> sys_UserRole = await _poeContext.Sys_UserRole.Where(x => x.UserID == p).ToListAsync();

        //        List<Sys_Role> sys_Role = await _poeContext.Sys_Role.ToListAsync();

        //        List<GetUserRolesResponse> getUserRolesResponse = sys_UserRole.Select(x => new GetUserRolesResponse {
        //            PartnerId = x.UserID,
        //            RoleId = x.RoleID,
        //            RoleName = sys_Role.Where(y => y.Id == x.RoleID).First().RoleName
        //        }).ToList();

        //        apiResult.Data = getUserRolesResponse;
        //    }
        //    catch (Exception ex)
        //    {
        //        apiResult.Code = -1;
        //        apiResult.Msg = ex.ToString();
        //    }
        //    return apiResult;
        //}

        //public async Task<ApiResult> AssignUserRoles(string id, AssignUserRolesRequest p)
        //{
        //    ApiResult apiResult = new ApiResult();
        //    try
        //    {
        //        List<Sys_UserRole> sys_UserRoleOld = _poeContext.Sys_UserRole.Where(x => x.UserID == id).ToList();

        //        List<Sys_UserRole> newUserRoles = p.RoleIds.Where(x => !sys_UserRoleOld.Exists(y => y.RoleID == x))
        //            .Select(x => new Sys_UserRole
        //            {
        //                Id = Guid.NewGuid().ToString(),
        //                UserID = id,
        //                RoleID = x
        //            }).ToList();
        //        DefaultEntityBase(ref newUserRoles);
        //        List<Sys_UserRole> deleteUserRoles = sys_UserRoleOld.Where(x => !p.RoleIds.Exists(y => y == x.RoleID)).ToList();
        //        _poeContext.RemoveRange(deleteUserRoles);
        //        await _poeContext.AddRangeAsync(newUserRoles);
        //        await _poeContext.SaveChangesAsync();
        //        apiResult.Data = new AssignUserRolesResponse { 
        //            PartnerId= id,
        //            RoleIds=p.RoleIds
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        apiResult.Code = -1;
        //        apiResult.Msg = ex.ToString();
        //    }
        //    return apiResult;
        //}

    }
}
