using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackEnd.DTOs.UserDiscounts;
using BackEnd.Exceptions;
using BackEnd.models;
using BackEnd.Repository;
using MongoDB.Bson.Serialization;


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
           return await _userDiscountRepo.Create(_mapper.Map<UserDiscount>(data));
        }

        public async Task<List<UserDiscountDto>> GetUserDiscount(string? userId)
        {
            if (userId == null) throw new UnAuthorizeException("error please login");
            var res = await _userDiscountRepo.GetUserVouchers(userId);
            var convert = res.Select(doc => BsonSerializer.Deserialize<UserDiscountDto>(doc)).ToList();
            return convert;
        }
    }
}