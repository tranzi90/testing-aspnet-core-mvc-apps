using AtmSimulator.Web.Controllers;
using AtmSimulator.Web.Models.Domain;
using AtmSimulator.Web.ViewModels;
using FluentAssertions;
using FluentAssertions.Equivalency;
using NSubstitute;
using NUnit.Framework;

namespace AtmSimulator.UnitTests.Controllers
{
    [TestFixture]
    public class HomeControllerTests : BaseTest
    {
        [Test]
        public void Home_viewmodel_is_returned()
        {
            // Arrange
            var atms = FakeAtms.Valid.Generate(10);

            var atmRepository = Substitute.For<IAtmRepository>();
            atmRepository.GetAll().Returns(atms);

            var expectedAtmsCount = atms.Count;

            var controller = new HomeController(atmRepository);

            // Act
            var indexResponse = controller.Index();

            // Assert
            var indexViewModel = indexResponse.GetFromViewResult();

            Assert.Multiple(() =>
            {
                atmRepository.Received(1).GetAll();

                indexViewModel.Atms.Should().BeEquivalentTo(atms, options => options.Using(new AtmViewModelComparer()));

                controller.ViewData.Should().Contain("TotalCount", expectedAtmsCount);

                var totalCountFromViewBag = (int)controller.ViewBag.TotalCount;
                totalCountFromViewBag.Should().Be(expectedAtmsCount);
            });
        }
    }

    public class AtmViewModelComparer : IEquivalencyStep
    {
        public bool CanHandle(
            IEquivalencyValidationContext context,
            IEquivalencyAssertionOptions config)
            => context.Subject is AtmViewModel
                && context.Expectation is Atm;

        public bool Handle(
            IEquivalencyValidationContext context,
            IEquivalencyValidator parent,
            IEquivalencyAssertionOptions config)
        {
            var atmViewModel = (AtmViewModel)context.Subject;
            var atm = (Atm)context.Expectation;

            atmViewModel.Id.Should().Be(atm.Id);
            atmViewModel.Balance.Should().Be(atm.Balance);

            return true;
        }
    }
}
