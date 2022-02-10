using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AtmSimulator.UnitTests
{
    public static class ActionResultExtensions
    {
        public static T GetInnerValue<T>(this ActionResult<T> actionResult)
        {
            actionResult.Result.Should().BeOfType<ObjectResult>();
            var objectResult = actionResult.Result as ObjectResult;

            objectResult.Value.Should().BeOfType<T>();
            var result = (T)objectResult.Value;

            return result;
        }

        public static bool IsOk(this ActionResult actionResult)
        {
            actionResult.Should().BeOfType<StatusCodeResult>();
            var objectResult = actionResult as StatusCodeResult;

            return objectResult.StatusCode == StatusCodes.Status200OK;
        }
    }
}
