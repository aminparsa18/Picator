using System.Text.Json;
using BenchmarkDotNet.Attributes;
using MemoryPack;
using Picator.Common.Data.Dtos.Api.Auth;

namespace Picator.Benchmark.Benchmarks;

/// <summary>
/// Small, flat payload (a couple of strings + inherited status fields) — roughly the shape
/// of a login/refresh response.
/// </summary>
[MemoryDiagnoser]
public class AuthResultSerializeBenchmarks
{
    private readonly AuthResult _model = new()
    {
        IsSuccess = true,
        Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIn0",
        RefreshToken = "9c858901-8a57-4791-81fe-4c455b099bc9.refresh-token-value",
    };

    [Benchmark(Baseline = true)]
    public byte[] MemoryPack() => MemoryPackSerializer.Serialize(_model);

    [Benchmark]
    public byte[] Json() => JsonSerializer.SerializeToUtf8Bytes(_model, PicatorJsonContext.Default.AuthResult);
}

[MemoryDiagnoser]
public class AuthResultDeserializeBenchmarks
{
    private byte[] _memoryPackBytes = [];
    private byte[] _jsonBytes = [];

    [GlobalSetup]
    public void Setup()
    {
        var model = new AuthResult
        {
            IsSuccess = true,
            Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIn0",
            RefreshToken = "9c858901-8a57-4791-81fe-4c455b099bc9.refresh-token-value",
        };

        _memoryPackBytes = MemoryPackSerializer.Serialize(model);
        _jsonBytes = JsonSerializer.SerializeToUtf8Bytes(model, PicatorJsonContext.Default.AuthResult);
    }

    [Benchmark(Baseline = true)]
    public AuthResult? MemoryPack() => MemoryPackSerializer.Deserialize<AuthResult>(_memoryPackBytes);

    [Benchmark]
    public AuthResult? Json() => JsonSerializer.Deserialize(_jsonBytes, PicatorJsonContext.Default.AuthResult);
}
