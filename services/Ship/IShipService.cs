using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.DTOs.Ship;

namespace BackEnd.services.Ship
{
    public interface IShipService
    {
        Task<object> getFeeShip(object data);
        Task<object> getProvince();
        Task<object> getDistrict(string provinceId);
        Task<object> getWard(string districtId);
        Task<ShipResponse> createOrder(decimal amout, string name, string phone, string address,string wardName,string districtName,string provinceName, string orderCode);
    }
}