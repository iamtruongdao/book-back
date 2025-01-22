using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using back.DTOs.Author;
using back.DTOs.Product;
using back.DTOs.Role;
using back.DTOs.User;
using back.models;

namespace back.helper
{
    public class MapperConfig:Profile
    {
        public MapperConfig()
        {
           CreateMap<ProductDTO,Product>();
            CreateMap<CreateProductDTO,Product>().ReverseMap();
            CreateMap<UpdateProductDTO,Product>().ReverseMap();
            CreateMap<AuthorDTO,Author>();
            CreateMap<CreateAuthorDTO,Author>().ReverseMap();
            CreateMap<UpdateAuthorDTO,Author>().ReverseMap();
            CreateMap<RegisterRequest,User>().ReverseMap();
            CreateMap<CreateRoleRequest,Role>().ReverseMap();
        }
    }
}