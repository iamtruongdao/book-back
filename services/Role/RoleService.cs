using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackEnd.DTOs.Role;
using BackEnd.models;
using Microsoft.AspNetCore.Identity;

namespace BackEnd.services
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly IMapper _mapper;
        public RoleService(RoleManager<Role> roleManager,IMapper mapper)
        {
            _mapper = mapper;
            _roleManager = roleManager;
        }
        public async Task<bool> CreateRole(CreateRoleRequest roleName)
        {
            var result = await _roleManager.CreateAsync( _mapper.Map<Role>(roleName)); 
            return result.Succeeded;
        }
    }
}