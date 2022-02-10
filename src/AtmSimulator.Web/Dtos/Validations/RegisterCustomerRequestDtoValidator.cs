using FluentValidation;

namespace AtmSimulator.Web.Dtos.Validations
{
    public class RegisterCustomerRequestDtoValidator : AbstractValidator<RegisterCustomerRequestDto>
    {
        public RegisterCustomerRequestDtoValidator()
        {
            RuleFor(x => x.CustomerName)
                .NotEmpty();

            RuleFor(x => x.Cash)
                .GreaterThanOrEqualTo(decimal.Zero);
        }
    }
}
