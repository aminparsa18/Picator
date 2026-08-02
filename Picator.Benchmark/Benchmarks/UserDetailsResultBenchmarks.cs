using System.Text.Json;
using BenchmarkDotNet.Attributes;
using MemoryPack;
using Picator.Common.Data.Dtos.Users;

namespace Picator.Benchmark.Benchmarks;

/// <summary>
/// Medium, flat payload — a handful of strings plus a value type, roughly the shape of a
/// user profile response.
/// </summary>
[MemoryDiagnoser]
public class UserDetailsResultSerializeBenchmarks
{
    private readonly UserDetailsResult _model = new()
    {
        Email = "amin.parsa@example.com",
        DisplayName = "Amin Parsa",
        Avatar = "https://cdn.picator.app/avatars/9c858901-8a57-4791-81fe-4c455b099bc9.png",
        Score = 4231,
    };

    [Benchmark(Baseline = true)]
    public byte[] MemoryPack() => MemoryPackSerializer.Serialize(_model);

    [Benchmark]
    public byte[] Json() => JsonSerializer.SerializeToUtf8Bytes(_model, PicatorJsonContext.Default.UserDetailsResult);
}

[MemoryDiagnoser]
public class UserDetailsResultDeserializeBenchmarks
{
    private byte[] _memoryPackBytes = [];
    private byte[] _jsonBytes = [];

    [GlobalSetup]
    public void Setup()
    {
        var model = new UserDetailsResult
        {
            Email = "amin.parsa@example.com",
            DisplayName = "Amin Parsa",
            Avatar = "https://cdn.picator.app/avatars/9c858901-8a57-4791-81fe-4c455b099bc9.png",
            Score = 4231,
        };

        _memoryPackBytes = MemoryPackSerializer.Serialize(model);
        _jsonBytes = JsonSerializer.SerializeToUtf8Bytes(model, PicatorJsonContext.Default.UserDetailsResult);
    }

    [Benchmark(Baseline = true)]
    public UserDetailsResult? MemoryPack() => MemoryPackSerializer.Deserialize<UserDetailsResult>(_memoryPackBytes);

    [Benchmark]
    public UserDetailsResult? Json() => JsonSerializer.Deserialize(_jsonBytes, PicatorJsonContext.Default.UserDetailsResult);
}
