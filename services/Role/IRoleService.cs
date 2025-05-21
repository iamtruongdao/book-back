using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.DTOs.Role;

namespace BackEnd.services
{
    public interface IRoleService
    {
        public Task<bool> CreateRole(CreateRoleRequest roleName);
    }
}