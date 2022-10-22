using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace MyWebApiServer.Controllers;

[ApiController]
[Route("[controller]")]
public class TokenController : ControllerBase
{
    [HttpPost("api/token")]
    public IActionResult GetTokenForCredentialsAsync()
    {
        Console.WriteLine("GetTokenForCredentialsAsync");

        // リクエストパラメータ経由でログイン情報を取得します。
        var resultPass = true; // DEBUG: 常にアカウント認証を承認します。

        if (resultPass)
        {
            var accessToken = GenerateToken("MyUser");
            return (IActionResult)Ok(accessToken);
        }
        return Unauthorized();
    }

    private string GenerateToken(string userId)
    {
        var key = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes("266031BF-D467-405D-89CE-564C92403479"));

        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId) };
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            "signalrdemo", "signalrdemo", claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}