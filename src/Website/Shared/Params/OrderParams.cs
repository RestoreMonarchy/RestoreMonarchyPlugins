﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Website.Shared.Models;

namespace Website.Shared.Params
{
    public class OrderParams
    {
        public int SellerId { get; set; }
        public string PaymentMethod { get; set; }

        public string BaseUrl { get; set; }
        public int BuyerId { get; set; }

        public List<OrderItemParams> Items { get; set; }

        [JsonIgnore]
        public UserModel Seller { get; set; }
        [JsonIgnore]
        public bool IsAgree { get; set; }
    }
}
