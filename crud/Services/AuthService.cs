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
            return GenerateToken(user);
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
        //private string GenerateToken(string username)
        //{
        //    var claims = new[]
        //    {
        //        new Claim(ClaimTypes.Name,username)
        //    };
        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:key"]));
        //    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        //    var token = new JwtSecurityToken(
        //        issuer: _configuration["Jwt:Issuer"],
        //        audience: _configuration["Jwt:Audience"],
        //        claims: claims,
        //        expires: DateTime.UtcNow.AddHours(10),
        //        signingCredentials: credentials
        //    );
        //    var tokenString = new JwtSecurityTokenHandler().CreateEncodedJwt(token);
        //    Console.WriteLine("Generated Token: " + tokenString); // Print token
        //    return tokenString;
        //    //return new JwtSecurityTokenHandler().WriteToken(token);
        //}
        private string GenerateToken(Auth user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Using CreateEncodedJwt properly requires different parameters
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(10),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.CreateEncodedJwt(tokenDescriptor);

            Console.WriteLine("Generated Token: " + tokenString); // Print token
            return tokenString;
        }
    }
}
