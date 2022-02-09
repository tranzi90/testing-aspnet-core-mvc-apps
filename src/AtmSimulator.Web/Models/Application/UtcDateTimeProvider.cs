using System;
using AtmSimulator.Web.Models.Domain;

namespace AtmSimulator.Web.Models.Application
{
    public sealed class UtcDateTimeProvider : IDateTimeProvider
    {
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}
