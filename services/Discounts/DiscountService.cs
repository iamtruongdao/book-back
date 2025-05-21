using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackEnd.DTOs.Discounts;
using BackEnd.Exceptions;
using BackEnd.models;
using BackEnd.Repository;
using BackEnd.Viewmodel;
using MongoDB.Driver;

namespace BackEnd.services.Discounts
{
    public class DiscountService : IDiscountService
    {
        private readonly IDiscountRepository _discountRepository;
        private readonly IMapper _mapper;

        public DiscountService(IDiscountRepository discountRepository, IMapper mapper)
        {
            _discountRepository = discountRepository;
            _mapper = mapper;
        }

        public async Task<Discount> CreateDiscount(CreateDiscountDto discount)
        {
            var founDiscount = await _discountRepository.FindOne(discount.Code!);
            if(founDiscount != null && founDiscount.IsActive)
            {
                throw new BadRequestException("Discount with this code already exists");
            }
            var result = await _discountRepository.CreateDiscount(_mapper.Map<Discount>(discount));
            return result;
        }

        public async Task<DeleteResult> DeleteDiscount(string id)
        {
            var result = await _discountRepository.DeleteDiscount(id);
            return result;
        }

        public async Task<PaginatedList<Discount>> GetAllDiscounts(int pageSize, int pageNumber)
        {
            var result = await _discountRepository.GetAllDiscounts(pageSize, pageNumber);
            return result;
        }

        public async Task<GetAmountResponse> GetAmount(GetAmountRequest request)
        {
            var discount = await _discountRepository.FindOne(request.CodeId!);
            if (discount is null) throw new NotFoundException("voucher not found");
            if (!discount.IsActive) throw new BadRequestException("voucher is expired");
            if (discount.StartDate > DateTime.UtcNow || discount.EndDate < DateTime.UtcNow) 
                throw new BadRequestException("voucher is expired");

            if (discount.MaxUsagePerUser > 0)
            {
                var userUsage = discount.UserUsage.FirstOrDefault(x => x == request.UserId);
                if (userUsage != null)
                {
                    var usageCount = discount.UserUsage.Count(x => x == userUsage);
                    if (usageCount >= discount.MaxUsagePerUser) 
                        throw new BadRequestException("voucher usage limit reached");
                }
            }

            decimal totalPrice = 0;
            if (discount.MinOrderValue > 0 || discount.ApplyTo == ApplyTo.Specific)
            {
                // Calculate total price based on ApplyTo
                if (discount.ApplyTo == ApplyTo.Specific)
                {
                    // Only include items whose ProductId is in discount.ProductIds
                    totalPrice = request.Items!
                        .Where(item => discount.ProductIds.Contains(item.ProductId!))
                        .Aggregate((decimal)0, (acc, item) => acc + item.Price * item.Quantity);
                }
                else // ApplyTo.All
                {
                    totalPrice = request.Items!
                        .Aggregate((decimal)0, (acc, item) => acc + item.Price * item.Quantity);
                }

                if (discount.MinOrderValue > 0 && totalPrice < discount.MinOrderValue) 
                    throw new BadRequestException($"voucher requires min order value of {discount.MinOrderValue}");
            }

            var amount = discount.Type == DiscountType.Percentage ?
                (totalPrice * discount.Value) / 100 : discount.Value;

            return new GetAmountResponse
            {
                TotalPrice = totalPrice,
                Amount = amount,
                FinalPrice = totalPrice - amount
            };
        }

        public async Task<ReplaceOneResult> UpdateDiscount(UpdateDiscountDto discount)
        {
            var result = await _discountRepository.Replace(_mapper.Map<Discount>(discount));
            if (result.MatchedCount == 0) throw new BadRequestException("error updating discount");
            return result;
        }

        public Task<UpdateResult> UpdateUserUse(string userId, string codeId)
        {
            var filter = Builders<Discount>.Filter.Eq(d => d.Code, codeId);
            var update = Builders<Discount>.Update.AddToSet(d => d.UserUsage, userId).Inc(x => x.UseCount, 1);
            return _discountRepository.Update(filter, update);
        }
    }
}