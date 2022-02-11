using System;
using System.Collections.Generic;
using System.Linq;
using AtmSimulator.Web.Database;
using AtmSimulator.Web.Models.Domain;
using CSharpFunctionalExtensions;

namespace AtmSimulator.Web.Models.Application
{
    public class SqlAtmRepository : IAtmRepository
    {
        private readonly AtmSimulatorDbContext _atmSimulatorDbContext;

        public SqlAtmRepository(AtmSimulatorDbContext atmSimulatorDbContext)
        {
            _atmSimulatorDbContext = atmSimulatorDbContext;
        }

        public Maybe<Atm> Get(Guid id)
        {
            var atmDal = _atmSimulatorDbContext.Atms.Find(id);

            if (atmDal is null)
            {
                return Maybe<Atm>.None;
            }

            return atmDal.ToDomain();
        }

        public IReadOnlyCollection<Atm> GetAll()
            => _atmSimulatorDbContext.Atms
            .ToArray()
            .Select(x => x.ToDomain())
            .ToArray();

        public Result Register(Atm atm)
        {
            try
            {
                var dal = atm.ToDal();

                _atmSimulatorDbContext.Atms.Add(dal);

                _atmSimulatorDbContext.SaveChanges();
            }
            catch (Exception e)
            {
                return Result.Failure(e.Message);
            }

            return Result.Success();
        }

        public Result Update(Atm atm)
        {
            try
            {
                var dal = _atmSimulatorDbContext.Atms.Find(atm.Id);

                if (dal is null)
                {
                    return Result.Failure("Can't find corresponding Atm to update.");
                }

                dal.Balance = atm.Balance;

                _atmSimulatorDbContext.Atms.Update(dal);

                _atmSimulatorDbContext.SaveChanges();
            }
            catch (Exception e)
            {
                return Result.Failure(e.Message);
            }

            return Result.Success();
        }
    }
}
