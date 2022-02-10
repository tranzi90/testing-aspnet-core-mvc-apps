using AtmSimulator.Web.Controllers;
using AtmSimulator.Web.Models.Application;
using CSharpFunctionalExtensions;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace AtmSimulator.UnitTests.Controllers
{
    [TestFixture]
    public class TransfersControllerTests : BaseTest
    {
        [Test]
        public void Customer_can_deposit_to_Atm()
        {
            // Arrange
            var paymentCardNumber = FakePaymentCardNumbers.Valid.Generate();
            var atmId = Faker.Random.Guid();
            var amount = decimal.One;

            var financialTransferSystemService = Substitute.For<IFinancialTransferSystemService>();

            financialTransferSystemService
                .DepositToAtm(
                    paymentCardNumber,
                    atmId,
                    amount)
                .Returns(Result.Success());

            var transfersController = new TransfersController(financialTransferSystemService);

            // Act
            var depositResult = transfersController.DepositToAtm(
                paymentCardNumber.ToString(),
                atmId,
                amount);

            // Assert
            depositResult.IsOk().Should().BeTrue();

            financialTransferSystemService
                .Received(1)
                .DepositToAtm(
                    paymentCardNumber,
                    atmId,
                    amount);
        }

        [Test]
        public void Customer_can_withdraw_from_Atm()
        {
            // Arrange
            var paymentCardNumber = FakePaymentCardNumbers.Valid.Generate();
            var atmId = Faker.Random.Guid();
            var amount = decimal.One;

            var financialTransferSystemService = Substitute.For<IFinancialTransferSystemService>();

            financialTransferSystemService
                .WithdrawFromAtm(
                    paymentCardNumber,
                    atmId,
                    amount)
                .Returns(Result.Success());

            var transfersController = new TransfersController(financialTransferSystemService);

            // Act
            var withdrawResult = transfersController.WithdrawFromAtm(
                atmId,
                paymentCardNumber.ToString(),
                amount);

            // Assert
            withdrawResult.IsOk().Should().BeTrue();

            financialTransferSystemService
                .Received(1)
                .WithdrawFromAtm(
                    paymentCardNumber,
                    atmId,
                    amount);
        }

        [Test]
        public void Customer_can_transfer_to_another_customer()
        {
            // Arrange
            var senderPaymentCardNumber = FakePaymentCardNumbers.Valid.Generate();
            var recipientPaymentCardNumber = FakePaymentCardNumbers.Valid.Generate();
            var amount = decimal.One;

            var financialTransferSystemService = Substitute.For<IFinancialTransferSystemService>();

            financialTransferSystemService
                .TransferToAnotherCustomer(
                    senderPaymentCardNumber,
                    recipientPaymentCardNumber,
                    amount)
                .Returns(Result.Success());

            var transfersController = new TransfersController(financialTransferSystemService);

            // Act
            var transferResult = transfersController.TransferToAnotherCustomer(
                senderPaymentCardNumber.ToString(),
                recipientPaymentCardNumber.ToString(),
                amount);

            // Assert
            transferResult.IsOk().Should().BeTrue();

            financialTransferSystemService
                .Received(1)
                .TransferToAnotherCustomer(
                    senderPaymentCardNumber,
                    recipientPaymentCardNumber,
                    amount);
        }
    }
}
