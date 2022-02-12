using System;
using System.Collections.Generic;
using AtmSimulator.Web.Models.Domain;
using CSharpFunctionalExtensions;

namespace AtmSimulator.FunctionalTests.Fakes
{
    public class FakeAtmRepository : IAtmRepository
    {
        private readonly List<Atm> _atms = new List<Atm>(0);

        public Maybe<Atm> Get(Guid id)
            => _atms.Find(x => x.Id == id) ?? Maybe<Atm>.None;

        public Result Register(Atm atm)
            => Result.Success().Tap(() => _atms.Add(atm));

        public Result Update(Atm atm)
            => Result.SuccessIf(_atms.Contains(atm), "Atm was not found.")
            .Tap(() =>
            {
                var index = _atms.IndexOf(atm);

                _atms[index] = atm;
            });

        public IReadOnlyCollection<Atm> GetAll()
            => _atms.AsReadOnly();
    }
}
