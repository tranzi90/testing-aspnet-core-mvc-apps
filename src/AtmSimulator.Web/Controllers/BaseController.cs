using System;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AtmSimulator.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        private ActionResult GenericResult(
            Result result,
            int successStatusCode = StatusCodes.Status200OK,
            int failureStatusCode = StatusCodes.Status400BadRequest)
            => result.Match<ActionResult>(
                () => StatusCode(successStatusCode),
                (error) => StatusCode(failureStatusCode, error));

        private ActionResult<TDto> GenericResult<T, TDto>(
            Result<T> result,
            Func<T, TDto> converter,
            int successStatusCode = StatusCodes.Status200OK,
            int failureStatusCode = StatusCodes.Status400BadRequest)
            => result.Match<ActionResult<TDto>, T>(
                (entity) => StatusCode(successStatusCode, converter(entity)),
                (error) => StatusCode(failureStatusCode, error));

        protected ActionResult OkUnprocessableResult(
            Result result)
            => GenericResult(
                result,
                StatusCodes.Status200OK,
                StatusCodes.Status422UnprocessableEntity);

        protected ActionResult<TDto> CreatedUnprocessableResult<T, TDto>(
            Result<T> result,
            Func<T, TDto> converter)
            => GenericResult(
                result,
                converter,
                StatusCodes.Status201Created,
                StatusCodes.Status422UnprocessableEntity);
    }
}
