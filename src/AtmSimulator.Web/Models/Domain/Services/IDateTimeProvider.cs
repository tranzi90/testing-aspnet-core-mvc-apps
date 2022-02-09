using System;

namespace AtmSimulator.Web.Models.Domain
{
    public interface IDateTimeProvider
    {
        public DateTimeOffset UtcNow { get; }
    }
}
