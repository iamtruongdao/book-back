using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.models;

namespace BackEnd.DTOs.Discounts
{
    public class CreateDiscountDto
    {
        public DiscountType Type { get; set; }
        public ApplyTo ApplyTo { get; set; } 
        public string? Code { get; set; }
        public string? Description { get; set; }
        public string? Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double Value { get; set; }
        public double MinOrderValue { get; set; }
        public List<string>? UserUsage { get; set; } 
        public List<string>? ProductIds { get; set; } 
        public int MaxUsage { get; set; }
        public int MaxUsagePerUser { get; set; }
    }
}