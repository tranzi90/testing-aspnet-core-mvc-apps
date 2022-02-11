using AtmSimulator.Web.Models.Domain;

namespace AtmSimulator.Web.Database
{
    public static class AtmDalConverter
    {
        public static AtmDal ToDal(this Atm atm)
            => new AtmDal
            {
                Id = atm.Id,
                Balance = atm.Balance,
            };

        public static Atm ToDomain(this AtmDal dal)
            => Atm.Create(
                dal.Id,
                dal.Balance);
    }
}
