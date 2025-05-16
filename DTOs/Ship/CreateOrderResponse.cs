using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BackEnd.DTOs.Ship
{
    public class CreateOrderResponse
    {
            [JsonPropertyName("code")]
            public int Code { get; set; }

            [JsonPropertyName("message")]
            public string? Message { get; set; }

            [JsonPropertyName("data")]
            public GhnOrderDataDto? Data { get; set; }

            [JsonPropertyName("message_display")]
            public string? MessageDisplay { get; set; }
    }
    public class GhnOrderDataDto
{
    [JsonPropertyName("order_code")]
    public string? OrderCode { get; set; }
}
}