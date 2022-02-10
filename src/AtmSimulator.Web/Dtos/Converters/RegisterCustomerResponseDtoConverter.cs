using AtmSimulator.Web.Models.Domain;

namespace AtmSimulator.Web.Dtos
{
    public static class RegisterCustomerResponseDtoConverter
    {
        public static RegisteredCustomerResponseDto ToDto(
            this Customer customer)
            => new RegisteredCustomerResponseDto
            {
                CustomerName = customer.Name.Name,
                Cash = customer.Cash,
                AccountId = customer.AccountId,
            };
    }
}
