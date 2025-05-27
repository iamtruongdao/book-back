using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackEnd.DTOs.UserDiscounts;
using BackEnd.Exceptions;
using BackEnd.models;
using BackEnd.Repository;
using BackEnd.Viewmodel;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;


namespace BackEnd.services.UserDiscounts
{
    public class UserDiscountService : IUserDiscountService
    {
        private readonly IMapper _mapper;
        private readonly IUserDiscountRepository _userDiscountRepo;
        public UserDiscountService(IMapper mapper, IUserDiscountRepository userDiscountRepo)
        {
            _userDiscountRepo = userDiscountRepo;
            _mapper = mapper;
        }
        public async Task<UserDiscount> CreateUserDiscount(SaveDiscountDto data)
        {
            var discountExist = await _userDiscountRepo.FindByDiscountId(data.DiscountId!,data.UserId!);
            if (discountExist != null) throw new BadRequestException("Bạn đã lưu voucher này rồi");
           return await _userDiscountRepo.Create(_mapper.Map<UserDiscount>(data));
        }

        public async Task<DeleteResult> DeleteVoucher(string id)
        {
            var result = await _userDiscountRepo.Delete(id);
            if (result.DeletedCount == 0) throw new BadRequestException("error! Đã có lỗi xảy ra ");
            return result;
        }

        public async Task<PaginatedList<UserDiscountDto>> GetUserDiscount(int pageSize,int pageNumber,string? userId)
        {
            if (userId == null) throw new UnAuthorizeException("error please login");
            var (list,total) = await _userDiscountRepo.GetUserVouchers(pageSize,pageNumber, userId);
            var convert = list.Select(doc => BsonSerializer.Deserialize<UserDiscountDto>(doc)).ToList();
            return new PaginatedList<UserDiscountDto>(convert,total,pageNumber,pageSize);
        }
    }
}