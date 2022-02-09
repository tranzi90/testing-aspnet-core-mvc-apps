using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;
using Dawn;
using Sprache;

namespace AtmSimulator.Web.Models.Domain
{
    public sealed class PaymentCardNumber : ValueObject
    {
        public const short MaximumNumberGroup = 10000;

        public const char Delimiter = '-';

        private static class PaymentCardParsingUtilities
        {
            public static readonly Parser<char> DelimiterParse = Parse.Char(Delimiter);

            public static readonly Parser<PaymentCardNumber> PaymentCardNumberParser
                = from firstGroup in Parse.Digit.Repeat(4)
                  from d1 in DelimiterParse
                  from secondGroup in Parse.Digit.Repeat(4)
                  from d2 in DelimiterParse
                  from thirdGroup in Parse.Digit.Repeat(4)
                  from d3 in DelimiterParse
                  from fourthGroup in Parse.Digit.Repeat(4)
                  select new PaymentCardNumber(
                      ToShort(firstGroup),
                      ToShort(secondGroup),
                      ToShort(thirdGroup),
                      ToShort(fourthGroup));

            private static short ToShort(IEnumerable<char> chars)
                => short.Parse(new string(chars.ToArray()));
        }

        private PaymentCardNumber(
            short firstGroup,
            short secondGroup,
            short thirdGroup,
            short fourthGroup)
        {
            FirstGroup = Guard.Argument(firstGroup, nameof(firstGroup)).Positive();
            SecondGroup = Guard.Argument(secondGroup, nameof(secondGroup)).Positive();
            ThirdGroup = Guard.Argument(thirdGroup, nameof(thirdGroup)).Positive();
            FourthGroup = Guard.Argument(fourthGroup, nameof(fourthGroup)).Positive();
        }

        public short FirstGroup { get; }

        public short SecondGroup { get;  }

        public short ThirdGroup { get; }

        public short FourthGroup { get; }

        public static PaymentCardNumber Create(
            short firstGroup,
            short secondGroup,
            short thirdGroup,
            short fourthGroup)
            => new PaymentCardNumber(
                firstGroup,
                secondGroup,
                thirdGroup,
                fourthGroup);

        public static CSharpFunctionalExtensions.Result Validate(
            short firstGroup,
            short secondGroup,
            short thirdGroup,
            short fourthGroup)
            => CSharpFunctionalExtensions.Result.Success()
            .Ensure(() => firstGroup > 0, "Payment card's number MUST be greater than zero.")
            .Ensure(() => secondGroup > 0, "Payment card's number MUST be greater than zero.")
            .Ensure(() => thirdGroup > 0, "Payment card's number MUST be greater than zero.")
            .Ensure(() => fourthGroup > 0, "Payment card's number MUST be greater than zero.");

        public static CSharpFunctionalExtensions.Result<PaymentCardNumber> TryCreate(
            short firstGroup,
            short secondGroup,
            short thirdGroup,
            short fourthGroup)
            => Validate(firstGroup, secondGroup, thirdGroup, fourthGroup)
            .Map(() => new PaymentCardNumber(firstGroup, secondGroup, thirdGroup, fourthGroup));

        public static CSharpFunctionalExtensions.Result<PaymentCardNumber> TryParse(string number)
        {
            if (string.IsNullOrWhiteSpace(number))
            {
                return CSharpFunctionalExtensions.Result.Failure<PaymentCardNumber>(
                    "Can't parse null, empty or whitespace string.");
            }

            var parsedResult = PaymentCardParsingUtilities.PaymentCardNumberParser.TryParse(number);

            return parsedResult.WasSuccessful
                ? CSharpFunctionalExtensions.Result.Success(parsedResult.Value)
                : CSharpFunctionalExtensions.Result.Failure<PaymentCardNumber>("Can't parse provided payment card's number.");
        }

        public override string ToString()
            => $"{FirstGroup:0000}{Delimiter}{SecondGroup:0000}{Delimiter}{ThirdGroup:0000}{Delimiter}{FourthGroup:0000}";

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return FirstGroup;
            yield return SecondGroup;
            yield return ThirdGroup;
            yield return FourthGroup;
        }
    }
}
