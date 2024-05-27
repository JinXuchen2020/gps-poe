using AutoMapper;
using POEMgr.Application.TransferModels;
using POEMgr.Repository;
using POEMgr.Repository.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POEMgr.Application
{
    public class MappingConfiguration : Profile
    {
        public MappingConfiguration() : base()
        {
            //CreateMap<Sys_User, AddUserRequest>();
            //CreateMap<AddUserRequest, Sys_User>();
            //CreateMap<Sys_User, UserResponse>();
        }
    }
}
