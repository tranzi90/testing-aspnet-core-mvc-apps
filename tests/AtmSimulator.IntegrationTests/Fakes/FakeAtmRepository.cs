using System;
using AtmSimulator.Web.Models.Domain;
using CSharpFunctionalExtensions;

namespace AtmSimulator.IntegrationTests.Fakes
{
    public class FakeAtmRepository : IAtmRepository
    {
        public Maybe<Atm> Get(Guid id)
            => Maybe<Atm>.None;

        public Result Register(Atm atm)
            => Result.Success();

        public Result Update(Atm atm)
            => Result.Success();
    }
}
