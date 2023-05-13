using Picator.Common.Data.Dtos.Data.Dtos.Api;
using MemoryPack;
using System.Collections.Generic;

namespace Picator.Common.Data.Dtos.Api;

/// <summary>
/// Api result returning in controllers.
/// </summary>
[MemoryPackable]
public partial class ApiResult
{
    /// <summary>
    /// Flag indicating api call has been successfull.
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Api result status code.
    /// </summary>
    public ApiResultStatusCode StatusCode { get; set; }

    /// <summary>
    /// Api result errrors.
    /// </summary>
    public IEnumerable<string>? Errors { get; set; }
}

/// <summary>
/// Generic typed api result returning in controllers.
/// </summary>
[MemoryPackable]
public sealed partial class ApiResult<TData> : ApiResult
    where TData : class
{
    /// <summary>
    /// Api result data.
    /// </summary>
    public TData? Data { get; set; }
}