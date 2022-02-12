using AtmSimulator.Web.Models.Domain;

namespace AtmSimulator.Web.Dtos
{
    public static class IssuedNewPaymentCardResponseDtoConverter
    {
        public static IssuedNewPaymentCardResponseDto ToDto(this PaymentCard paymentCard)
            => new IssuedNewPaymentCardResponseDto
            {
                PaymentCardNumber = paymentCard.Number.ToString(),
            };
    }
}
