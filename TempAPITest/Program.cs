using Microsoft.AspNetCore.Identity;
using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Users;
using Picator.Game.Extensions;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

builder.Services.AddHttpClient("webapi", cfg =>
{
    cfg.BaseAddress = new Uri("http+https://picator-api/api/v1/");// new Uri(builder.Configuration["WebApi:BaseAddress"]);
    cfg.AddDefaultMessagePackAcceptHeader();
}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
});

// Add services to the container.
builder.Services.AddProblemDetails();


var app = builder.Build();

app.MapDefaultEndpoints();
app.UseRouting();

app.MapGet("/login", async (IHttpClientFactory httpClientFactory, HttpContext ctx) =>
{
    var client = httpClientFactory.CreateClient("webapi");
    var based = client.BaseAddress;
    var res = await client.PostAsMemoryPackAsync("users/login", new UserLoginRequest { Username = "apar0133@student.monash.edu", Password = "54Delta45!" }, true);
    return await res.Content.ReadAsStringAsync();
});

app.MapGet("/register", async (IHttpClientFactory httpClientFactory, HttpContext ctx) =>
{
    var client = httpClientFactory.CreateClient("webapi");
    var based = client.BaseAddress;
    var res = await client.PostAsMemoryPackAsync("users/register", new RegisterUserRequest { UserName = "apar0133@student.monash.edu", Password = "54Delta45!" }, true);
    return await res.Content.ReadAsStringAsync();
});

app.MapGet("/details", async (IHttpClientFactory httpClientFactory, HttpContext ctx) =>
{
    var client = httpClientFactory.CreateClient("webapi");
    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJyb2xlIjoiUGxheWVyIiwianRpIjoiMjNiYTEzNzYtN2IxOS00YTQ0LThmN2ItZmQ0NGJhZGUxOGY2IiwidW5pcXVlX25hbWUiOiJjYWY4MGY5ZC05MjgzLTRhOTgtODQzOS1mY2U0ZTNiMjYxMTYiLCJuYmYiOjE3NjQ1OTkzMzQsImV4cCI6MTc2NDY0MjUzNCwiaWF0IjoxNzY0NTk5MzM0fQ.z-LIJvHNfyWiFRvhiRTYFFzGd91rz1htX-etgdpit9Q");
   await Task.Delay(2000);
    var res = await client.GetFromMemoryPackAsync<ApiResult<UserDetailsResult>>("users");
    return res;
});
app.Run();
