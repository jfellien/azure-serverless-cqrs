using System;
using Newtonsoft.Json;

namespace devCrowd.ServerlessCQRS.Core.Projections.Sales
{
    public class OrderItem : SalesProjection
    {
        public const string DOCUMENT_TYPE = "orderItem";
        
        public OrderItem() : base(Guid.NewGuid().ToString(), DOCUMENT_TYPE) { }
        
        public OrderItem(string id) : base(id, DOCUMENT_TYPE) { }
        
        [JsonProperty("number")]
        public string OrderNumber { get; set; }
        [JsonProperty("date")]
        public string OrderDate { get; set; }
        [JsonProperty("amount")]
        public int Amount { get; set; }
        [JsonProperty("toppings")]
        public string Toppings { get; set; }
        [JsonProperty("customer")]
        public string CustomerName { get; set; }
        [JsonProperty("customerAddress")]
        public string CustomerAddress { get; set; }
        [JsonProperty("orderId")]
        public string OrderId { get; set; }
    }
}