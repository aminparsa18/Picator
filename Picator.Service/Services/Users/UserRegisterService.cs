using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Users;
using Picator.Common.Helpers;
using Picator.Data;
using Picator.Data.Mappers;
using Picator.Entities.Identity;
using Picator.Service.Contracts;
using Picator.Service.Contracts.Users;
using System.Data;

namespace Picator.Service.Services.Users;

public class UserRegisterService : IUserRegisterService
{
    private readonly IEmailSender _emailSender;
    private readonly IHttpContextAccessor _http;
    private readonly ApplicationDbContext _dbConnection;
    private readonly IValidator<RegisterUserRequest> _validator;
    private readonly UserManager<User> _userManager;

    public UserRegisterService(IEmailSender emailSender, IHttpContextAccessor httpContextAccessor, ApplicationDbContext dbConnection, IValidator<RegisterUserRequest> validator, UserManager<User> userManager)
    {
        _emailSender = emailSender;
        _http = httpContextAccessor;
        _dbConnection = dbConnection;
        _validator = validator;
        _userManager = userManager;
    }

    public async Task<ApiResult> Register(RegisterUserRequest request)
    {
        await _emailSender.SendEmailAsync("aminparsa18@gmail.com", "subj", "msggggg");
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return new ApiResult
            {
                StatusCode = ApiResultStatusCode.BadRequest,
                Errors = validationResult.Errors.Select(e => e.ErrorMessage)
            };
        var users = await _dbConnection.Users.FromSql($"SELECT TOP 1 * FROM [Users] WHERE Username = {request.UserName}").ToListAsync();
        if (users.Count != 0)
        {
            return new ApiResult()
            {
                StatusCode = ApiResultStatusCode.Conflict,
                Errors = ["User already exist."]
            };
        }

        var user = new UserMapper().RegisterRequestToUser(request);
        user.Email = user.UserName;
        user.Id = Guid.NewGuid(); ;
        user.Code = RandomHelper.CreateRandomText(10);
        user.Score = 100;
        var createdUser = await _userManager.CreateAsync(user, request.Password ?? "");
        if (!createdUser.Succeeded)
        {
            return new ApiResult()
            {
                StatusCode = ApiResultStatusCode.LogicError,
                Errors = createdUser.Errors.Select(x => x.Description)
            };
        }

        await _userManager.AddToRoleAsync(user, Constants.PlayerRole);
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        await SendEmailConfirmationAsync(user.Email, token);
        return new ApiResult()
        {
            IsSuccess = true
        };
    }

    private string GetBaseUrl()
    {
        var req = _http.HttpContext?.Request;

        if (req == null)
            return null!; // we're not in an HTTP request

        return $"{req.Scheme}://{req.Host}{req.PathBase}";
    }

    public async Task SendEmailConfirmationAsync(string email, string token)
    {
        var encodedEmail = Uri.EscapeDataString(email);
        var encodedToken = Uri.EscapeDataString(token);
        var baseurl = GetBaseUrl();
        // API endpoint you want to hit when user clicks the button
        var confirmUrl =
            $"{baseurl}/api/v1/users/confirm-email?email={encodedEmail}&token={encodedToken}";

        var subject = "Confirm your email";

        var body = BuildConfirmationTemplate(confirmUrl);

        await _emailSender.SendEmailAsync(email, subject, body, isHtml: true);
    }

    private static string BuildConfirmationTemplate(string confirmUrl)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"" />
    <title>Confirm your email</title>
</head>
<body style=""font-family: Arial, sans-serif; background-color:#f5f5f5; padding:20px;"">
    <div style=""max-width:500px; margin:0 auto; background:#ffffff; padding:24px; border-radius:8px;"">
        <h2 style=""margin-top:0;"">Confirm your email</h2>
        <p>Thanks for signing up. Please confirm your email address by clicking the button below:</p>
        <p style=""text-align:center; margin:32px 0;"">
            <a href=""{confirmUrl}""
               style=""background-color:#2563eb; color:#ffffff; text-decoration:none; padding:12px 24px; border-radius:999px; display:inline-block;"">
                Confirm Email
            </a>
        </p>
        <p>If the button doesn't work, copy and paste this link into your browser:</p>
        <p style=""word-break:break-all; font-size:12px;"">
            <a href=""{confirmUrl}"">{confirmUrl}</a>
        </p>
    </div>
</body>
</html>";
    }
}