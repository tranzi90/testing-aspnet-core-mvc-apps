using System.Collections.Generic;
using CSharpFunctionalExtensions;
using Dawn;

namespace AtmSimulator.Web.Models.Domain
{
    public sealed class CustomerName : ValueObject
    {
        private CustomerName(string name)
        {
            Name = Guard.Argument(name, nameof(name)).NotNull().NotEmpty().NotWhiteSpace();
            NormalizedName = Name.ToUpperInvariant();
        }

        public string Name { get; }

        public string NormalizedName { get; }

        public override string ToString()
            => $"🏷️: Name=[{Name}]. NormalizedName=[{NormalizedName}].";

        public static CustomerName Create(string name)
            => new CustomerName(name);

        public static Result Validate(string name)
            => Result.FailureIf(string.IsNullOrWhiteSpace(name), "Customer's name MUST not be null, empty or whitespace.");

        public static Result<CustomerName> TryCreate(string name)
            => Validate(name)
            .Map(() => new CustomerName(name));

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return NormalizedName;
        }
    }
}
