using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.DTOs.Discounts;
using BackEnd.models;
using BackEnd.Viewmodel;
using MongoDB.Driver;
namespace BackEnd.services.Discounts
{
    public interface IDiscountService
    {
        Task<Discount> CreateDiscount(CreateDiscountDto discount);
        Task<PaginatedList<Discount>> GetAllDiscounts(int pageSize, int pageNumber);
        Task<DeleteResult> DeleteDiscount(string id);
        Task<GetAmountResponse> GetAmount(GetAmountRequest request);
        Task<UpdateResult> UpdateUserUse(string userId, string codeId);
        Task<ReplaceOneResult> UpdateDiscount(UpdateDiscountDto discount);
        
    }
}