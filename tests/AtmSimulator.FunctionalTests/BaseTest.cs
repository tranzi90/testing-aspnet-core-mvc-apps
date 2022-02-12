using System;
using System.Text.Json;
using Bogus;
using NUnit.Framework;

[assembly: Parallelizable(ParallelScope.Fixtures)]
[assembly: LevelOfParallelism(4)]

namespace AtmSimulator.FunctionalTests
{
    public abstract class BaseTest
    {
        protected Faker Faker { get; private set; }

        protected JsonSerializerOptions SerializerOptions { get; private set; }

        [SetUp]
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
