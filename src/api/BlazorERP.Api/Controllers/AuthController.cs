using BlazorERP.Application.Models;
using BlazorERP.Application.Services.Interfaces;
using BlazorERP.Contracts.Requests;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;
using System.Text;
using BlazorERP.Contracts.Responses;

namespace BlazorERP.Api.Controllers;

[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private const string TokenSecret = "ForTheLoveOfGodStoreAndLoadThisSecurely"; // TODO: Load from appSettings
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(8); // TODO: Load from appSettings
    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost(ApiEndpoints.Auth.PerformLogin)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken token)
    {
        // First try local login
        var user = await _userService.GetByUsernameAsync(request.Username);

        if (user is not null)
        {
            PasswordHasher<User> hasher = new();

            string passwordHashed = hasher.HashPassword(user, request.Password + user.Salt);

            PasswordVerificationResult result = hasher.VerifyHashedPassword(user, user.Password, request.Password + user.Salt);
            // We handle login whenever we have a user object. So when it fails we need to set it to null
            if (result is PasswordVerificationResult.Failed)
            {
                user = null;
            }
        }

        // TODO: LDAP Login

        if (user is not null)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Sub, user.Email),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(ClaimTypes.Name, user.Firstname), // TODO: Fullname
                new("userId", user.UserId.ToString()),
            };

            // Generate JWT-Token

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(TokenSecret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(TokenLifetime),
                Issuer = "https://id.company.com", // TODO: Load from appsettings
                Audience = "https://movies.company.com", // TODO: Load from appsettings
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var jwtToken = tokenHandler.CreateToken(tokenDescriptor);

            var jwt = tokenHandler.WriteToken(jwtToken);
            return Ok(new LoginResponse
            {
                Token = jwt,
                Success = true,
                Message = string.Empty
            });
        }

        return NotFound(new LoginResponse
        {
            Token = null,
            Success = false,
            Message = "Wrong Username or password"
        });
    }
}
