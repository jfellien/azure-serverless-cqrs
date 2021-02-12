namespace devCrowd.ServerlessCQRS.Contexts.Sales.CommandHandler.Models
{
    public class Order
    {
        public string ClientId { get; set; }
        public string Paste { get; set; }
        public string Tomatoes { get; set; }
        public string Cheese { get; set; }
        public int Amount { get; set; }
    }
}