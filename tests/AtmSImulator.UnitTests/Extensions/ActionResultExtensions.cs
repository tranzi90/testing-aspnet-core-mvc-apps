using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AtmSimulator.UnitTests
{
    public static class ActionResultExtensions
    {
        public static T GetFromObjectResult<T>(this ActionResult<T> actionResult)
        {
            actionResult.Result.Should().BeOfType<ObjectResult>();
            var objectResult = actionResult.Result as ObjectResult;

            objectResult.Value.Should().BeOfType<T>();
            var result = (T)objectResult.Value;

            return result;
        }

        public static T GetFromViewResult<T>(this ActionResult<T> actionResult)
        {
            actionResult.Result.Should().BeOfType<ViewResult>();
            var viewResult = actionResult.Result as ViewResult;

            viewResult.Model.Should().BeOfType<T>();
            var result = (T)viewResult.Model;

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
