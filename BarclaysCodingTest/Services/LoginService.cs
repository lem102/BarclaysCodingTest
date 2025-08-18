using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BarclaysCodingTest.Dtos;
using BarclaysCodingTest.Entities;
using BarclaysCodingTest.Database.Repository;
using BarclaysCodingTest.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace BarclaysCodingTest.Services;

public class LoginService(
    IPasswordHasher<UserEntity> passwordHasher,
    IConfiguration configuration,
    IRepository<UserEntity> repository
) : ILoginService
{
    public Result<string> Login(LoginUserRequest request)
    {
        var nullableUser = repository.GetAll().FirstOrDefault(u => u.Name == request.Name);

        if (nullableUser is not UserEntity user)
        {
            return Errors.IncorrectLoginDetails();
        }

        var hasValidPassword = passwordHasher.VerifyHashedPassword(
            user,
            user.Password,
            request.Password
        ) == PasswordVerificationResult.Success;

        if (!hasValidPassword)
        {
            return Errors.IncorrectLoginDetails();
        }

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token).ToString();
    }
}
