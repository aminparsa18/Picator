using FastEndpoints;
using MemoryPack;
using System.Text.Json;

namespace Picator.Configuration;

public class MemoryPackRequestBinder<TRequest> : RequestBinder<TRequest> where TRequest : notnull, new()
{
    public override async ValueTask<TRequest> BindAsync(BinderContext ctx, CancellationToken ct)
    {
        var req = ctx.HttpContext.Request;
        var referer = req.Headers.Referer.FirstOrDefault();
        if (!referer?.Contains("/swagger") ?? true)
        {
            try
            {
                var ss = req.ContentLength;
                var obj = await MemoryPackSerializer.DeserializeAsync<TRequest>(req.Body, cancellationToken: ct);
                return obj!;
            }
            catch (MemoryPackSerializationException ex)
            {
                if (req.ContentLength == null || req.ContentLength == 0)
                    return default;
                throw new InvalidOperationException("request is not in correct scheme", ex);
            }
        }
        ctx.HttpContext.Items["IsSwaggerRequest"] = true;
        if (req.ContentLength == null || req.ContentLength == 0)
            return default;
        var jsonOptions = ctx.JsonSerializerContext?.Options
                             ?? new JsonSerializerOptions
                             {
                                 PropertyNameCaseInsensitive = true
                             };

        var jsonResult = await JsonSerializer.DeserializeAsync<TRequest>(req.Body, jsonOptions, ct);
        return jsonResult!;
    }
}