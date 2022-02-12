using TechTalk.SpecFlow;

namespace AtmSimulator.FunctionalTests.Bdd.Steps
{
    [Binding]
    [Scope(Feature = "DepositToAtm")]
    public class DepositToAtmSteps : TransferFeaturesSteps
    {
        [When(@"(.*) deposit to ATM (.*)")]
        public async System.Threading.Tasks.Task DepositToAtm(string name, decimal amount)
        {
            await DepositToAtmInner(name, amount);
        }
    }
}
