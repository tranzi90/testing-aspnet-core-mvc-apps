using System;
using System.Text.Json;
using Bogus;
using NUnit.Framework;
using TechTalk.SpecFlow;

[assembly: Parallelizable(ParallelScope.Fixtures)]
[assembly: LevelOfParallelism(4)]

namespace AtmSimulator.FunctionalTests.Bdd
{
    public abstract class BaseTest
    {
        protected Faker Faker { get; private set; }

        protected JsonSerializerOptions SerializerOptions { get; private set; }

        [BeforeScenario(Order = 0)]
        public void SetupBeforeEachTest()
        {
            const int seed = 1234;

            Randomizer.Seed = new Random(seed);

            Faker = new Faker("uk");

            SerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            };
        }
    }
}
