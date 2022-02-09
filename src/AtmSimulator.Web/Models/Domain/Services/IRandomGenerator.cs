using System;

namespace AtmSimulator.Web.Models.Domain
{
    public interface IRandomGenerator
    {
        short NextPositiveShort();

        Guid NewGuid();
    }
}
