using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.DTOs.UserDiscounts;
using BackEnd.models;

namespace BackEnd.services.UserDiscounts
{
    public interface IUserDiscountService
    {
        Task<UserDiscount> CreateUserDiscount(SaveDiscountDto data);
        Task<List<UserDiscountDto>> GetUserDiscount(string? userId);
    }
}