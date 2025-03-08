using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using back.DTOs.Auth;
using back.DTOs.Author;
using back.DTOs.Categories;
using back.DTOs.Inventory;
using back.DTOs.Product;
using back.DTOs.Role;
using back.DTOs.User;
using back.models;
using Microsoft.AspNetCore.Identity;

namespace back.helper
{
    public class MapperConfig:Profile
    {
       
        public MapperConfig()
        { 
            CreateMap<ProductDTO, Product>();
            CreateMap<CreateProductDTO, Product>().ReverseMap();
            CreateMap<UpdateProductDTO, Product>().ReverseMap();
            CreateMap<AuthorDTO, Author>();
            CreateMap<CreateAuthorDTO, Author>().ReverseMap();
            CreateMap<UpdateAuthorDTO, Author>().ReverseMap();
            CreateMap<RegisterRequest, User>().ReverseMap();
            CreateMap<CreateRoleRequest, Role>().ReverseMap();
            CreateMap<AddStockToInventoryDTO, Inventory>().ForAllMembers(otp => otp.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<CreateCatDTO, Category>();
            CreateMap<UpdateCatDTO, Category>();
            CreateMap<User, UserResponse>().ForMember(dest => dest.IsLocked, opt => opt.MapFrom(src => src.LockoutEnd.HasValue && src.LockoutEnd > DateTime.UtcNow));

        }
        
    }
}