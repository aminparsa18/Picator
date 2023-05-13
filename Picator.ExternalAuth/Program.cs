using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddAuthentication(o =>
{
    o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
    .AddCookie(c =>
    {
        c.LoginPath = "/mobileauth/google-login";
    })
    .AddGoogle(GoogleDefaults.AuthenticationScheme, g =>
    {
        g.ClientId = "890605411237-srmckel8r27meeu680c1b1qgnl3urkr1.apps.googleusercontent.com";
        g.ClientSecret = "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";
        //g.SaveTokens = true;
    });

var app = builder.Build();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();