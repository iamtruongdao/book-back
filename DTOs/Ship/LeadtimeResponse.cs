using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.DTOs.Ship
{
    public class LeadtimeResponse
    {
        public int Code { get; set; }
        public string? Message { get; set; }
        public LeadtimeData? Data { get; set; }
    }
    public class LeadtimeData
    {
        public long Leadtime { get; set; }
        public long OrderDate { get; set; }
    }
}