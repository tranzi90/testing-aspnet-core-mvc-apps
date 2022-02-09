using System;
using AtmSimulator.Web.Models.Domain;

namespace AtmSimulator.Web.Models.Application
{
    public class BasicRandomGenerator : IRandomGenerator
    {
        private static readonly Random _random = new Random();

        public short NextPositiveShort()
            => (short)_random.Next(1, short.MaxValue);

        public Guid NewGuid() => Guid.NewGuid();
    }
}
