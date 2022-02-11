using System;

namespace AtmSimulator.Web.Database
{
    public class CustomerDal
    {
        public string Name { get; set; }

        public decimal Cash { get; set; }

        public Guid AccountId { get; set; }
    }
}
