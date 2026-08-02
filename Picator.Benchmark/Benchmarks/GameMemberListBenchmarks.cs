using System.Text.Json;
using BenchmarkDotNet.Attributes;
using MemoryPack;
using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Data.Dtos.Api;
using Picator.Common.Data.Dtos.GameMembers;
using Picator.Common.Data.Enums;

namespace Picator.Benchmark.Benchmarks;

internal static class GameMemberListFactory
{
    public static ApiResult<List<GameMemberResult>> Create(int memberCount)
    {
        var members = new List<GameMemberResult>(memberCount);
        for (var i = 0; i < memberCount; i++)
        {
            members.Add(new GameMemberResult
            {
                Id = Guid.NewGuid(),
                DisplayName = $"Player {i}",
                Avatar = $"https://cdn.picator.app/avatars/{i}.png",
                Score = i * 17,
                Status = (PlayerStatus)(i % 3),
            });
        }

        return new ApiResult<List<GameMemberResult>>
        {
            IsSuccess = true,
            StatusCode = ApiResultStatusCode.Success,
            Data = members,
        };
    }
}

/// <summary>
/// Nested/collection payload — an <see cref="ApiResult{TData}"/> wrapping a list of game
/// members, roughly the shape of a "get game members" endpoint response.
/// </summary>
[MemoryDiagnoser]
public class GameMemberListSerializeBenchmarks
{
    [Params(10, 100)]
    public int MemberCount { get; set; }

    private ApiResult<List<GameMemberResult>> _model = null!;

    [GlobalSetup]
    public void Setup() => _model = GameMemberListFactory.Create(MemberCount);

    [Benchmark(Baseline = true)]
    public byte[] MemoryPack() => MemoryPackSerializer.Serialize(_model);

    [Benchmark]
    public byte[] Json() => JsonSerializer.SerializeToUtf8Bytes(_model, PicatorJsonContext.Default.ApiResultListGameMemberResult);
}

[MemoryDiagnoser]
public class GameMemberListDeserializeBenchmarks
{
    [Params(10, 100)]
    public int MemberCount { get; set; }

    private byte[] _memoryPackBytes = [];
    private byte[] _jsonBytes = [];

    [GlobalSetup]
    public void Setup()
    {
        var model = GameMemberListFactory.Create(MemberCount);
        _memoryPackBytes = MemoryPackSerializer.Serialize(model);
        _jsonBytes = JsonSerializer.SerializeToUtf8Bytes(model, PicatorJsonContext.Default.ApiResultListGameMemberResult);
    }

    [Benchmark(Baseline = true)]
    public ApiResult<List<GameMemberResult>>? MemoryPack() =>
        MemoryPackSerializer.Deserialize<ApiResult<List<GameMemberResult>>>(_memoryPackBytes);

    [Benchmark]
    public ApiResult<List<GameMemberResult>>? Json() =>
        JsonSerializer.Deserialize(_jsonBytes, PicatorJsonContext.Default.ApiResultListGameMemberResult);
}
