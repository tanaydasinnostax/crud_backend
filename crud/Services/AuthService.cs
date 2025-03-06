using crud.Models;

using crud.Repositories;
using BCrypt.Net;
using crud.Services;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace crud.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;
        public AuthService(IAuthRepository authRepository, IConfiguration configuration)
        {
            _authRepository = authRepository;
            _configuration = configuration;
        }
        public async Task<string> LoginAsync(string username, string password)
        {
            var user = await _authRepository.GetByUsernameAsync(username);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.Please register first.");
            }
            else if (!BCrypt.Net.BCrypt.Verify(password, user.SecuredPassword))
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }
            return GenerateToken(username);
        }
        public async Task<string> SignUpAsync(string username, string email, string password)
        {
            var existingUser = await _authRepository.GetByUsernameAsync(username);
            if (existingUser != null)
            {
                throw new InvalidOperationException("User Already Exist");
            }
            var user = new Auth
            {
                Username = username,
                Email = email,
                SecuredPassword = BCrypt.Net.BCrypt.HashPassword(password)
            };
            await _authRepository.AddUserAsync(user);
            return "User Registerd Successfully";
        }
        private string GenerateToken(string username)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name,username)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
