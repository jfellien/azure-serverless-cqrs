using System;

namespace devCrowd.ServerlessCQRS.Core.Projections.Sales
{
    public class Customer : SalesProjection
    {
        public const string DOCUMENT_TYPE = "customer";

        public Customer(): base(Guid.NewGuid().ToString(), DOCUMENT_TYPE) { }
        
        public Customer(string id) : base(id, DOCUMENT_TYPE) { }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
    }
}