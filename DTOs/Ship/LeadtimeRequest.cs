using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BackEnd.DTOs.Ship
{
    public class LeadtimeRequest
    {
        [JsonPropertyName("from_district_id")]
        public int FromDistrictId { get; set; }

        [JsonPropertyName("from_ward_code")]
        public string? FromWardCode { get; set; }

        [JsonPropertyName("to_district_id")]
        public int ToDistrictId { get; set; }

        [JsonPropertyName("to_ward_code")]
        public string? ToWardCode { get; set; }

        [JsonPropertyName("service_id")]
        public int ServiceId { get; set; }
    }
}