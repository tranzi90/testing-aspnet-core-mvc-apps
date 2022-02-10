using System;

namespace AtmSimulator.Web.Dtos
{
    public class RegisteredCustomerResponseDto
    {
        public string CustomerName { get; set; }

        public decimal Cash { get; set; }

        public Guid AccountId { get; set; }
    }
}
