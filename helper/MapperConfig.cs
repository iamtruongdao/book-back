using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackEnd.DTOs.Auth;
using BackEnd.DTOs.Author;
using BackEnd.DTOs.Categories;
using BackEnd.DTOs.Inven;
using BackEnd.DTOs.Product;
using BackEnd.DTOs.Role;
using BackEnd.DTOs.User;
using BackEnd.models;
using BackEnd.DTOs.Discounts;
using BackEnd.DTOs.Posts;
using BackEnd.DTOs.Tag;
using BackEnd.DTOs.UserDiscounts;



namespace BackEnd.helper
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
            CreateMap<CreatePostDto, Post>();
            CreateMap<UpdatePostDto, Post>();
            CreateMap<CreateTagDto, Tags>();
            CreateMap<UpdateTagDto, Tags>();
            CreateMap<CreateDiscountDto, Discount>();
            CreateMap<UpdateDiscountDto, Discount>();
            CreateMap<SaveDiscountDto, UserDiscount>();

        }
        
    }
}