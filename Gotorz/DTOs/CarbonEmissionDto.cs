﻿using System.Text.Json.Serialization;

namespace Gotorz.DTOs
{
    public class CarbonEmissions
    {
        [JsonPropertyName("this_flight")]
        public int ThisFlight { get; set; }

        [JsonPropertyName("typical_for_this_route")]
        public int TypicalForThisRoute { get; set; }

        [JsonPropertyName("difference_percent")]
        public int DifferencePercent { get; set; }
    }
}

