﻿using System.Text.Json.Serialization;

namespace Gotorz.DTOs
{
    public class HotelInfo
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("address")]
        public string Address { get; set; }
    }
}
