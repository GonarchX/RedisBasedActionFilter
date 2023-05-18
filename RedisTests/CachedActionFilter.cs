using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RedisTests.CacheService;

namespace RedisTests;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class CachedActionFilter : Attribute, IAsyncActionFilter
{
    private readonly double _timeToLiveInSeconds;

    public CachedActionFilter(double timeToLiveInSeconds)
    {
        _timeToLiveInSeconds = timeToLiveInSeconds;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var cacheService = context.HttpContext.RequestServices.GetService<ICacheService>()!;
        var cacheKey = GenerateCacheKey(context.HttpContext.Request);
        var cachedValue = await cacheService.GetCachedResponse(cacheKey, context.HttpContext.RequestAborted);
        if (!string.IsNullOrEmpty(cachedValue))
        {
            context.Result = new ContentResult
            {
                Content = cachedValue,
                ContentType = "application/json",
                StatusCode = (int)HttpStatusCode.OK
            };
            return;
        }

        var executedContext = await next();

        if (!executedContext.ExceptionHandled &&
            executedContext.Result is ObjectResult result &&
            IsSuccessHttpStatusCode(result))
        {
            await cacheService.CacheResponse(cacheKey, result.Value, _timeToLiveInSeconds,
                context.HttpContext.RequestAborted);
        }
    }

    private bool IsSuccessHttpStatusCode(ObjectResult objectResult) => 
        objectResult.StatusCode is >= 200 and < 400;

    private string GenerateCacheKey(HttpRequest request)
    {
        var keyBuilder = new StringBuilder();
        keyBuilder.Append(request.Path);
        foreach (var (key, value) in request.Query)
        {
            keyBuilder.Append($"|{key}-{value}");
        }

        return keyBuilder.ToString();
    }
}