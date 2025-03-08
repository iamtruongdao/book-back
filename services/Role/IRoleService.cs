using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using back.DTOs.Role;

namespace back.services
{
    public interface IRoleService
    {
        public Task<bool> CreateRole(CreateRoleRequest roleName);
    }
}