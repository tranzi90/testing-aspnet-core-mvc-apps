using System;
using System.Collections.Generic;
using System.Linq;
using AtmSimulator.Web.Models.Domain;
using CSharpFunctionalExtensions;

namespace AtmSimulator.IntegrationTests.Fakes
{
    public class FakeAtmRepository : IAtmRepository
    {
        private readonly List<Atm> _atms;

        public FakeAtmRepository(AtmFakeData atmFakeData)
        {
            _atms = atmFakeData.Valid.Generate(10);
        }

        public Maybe<Atm> Get(Guid id)
            => _atms.FirstOrDefault(x => x.Id == id) ?? Maybe<Atm>.None;

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
