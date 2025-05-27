using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.DTOs.UserDiscounts;
using BackEnd.models;
using BackEnd.Viewmodel;
using MongoDB.Driver;

namespace BackEnd.services.UserDiscounts
{
    public interface IUserDiscountService
    {
        Task<UserDiscount> CreateUserDiscount(SaveDiscountDto data);
        Task<PaginatedList<UserDiscountDto>> GetUserDiscount(int pageSize, int pageNumber, string? userId);
        Task<DeleteResult> DeleteVoucher(string id);
    }
}