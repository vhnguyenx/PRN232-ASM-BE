using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using QE180082_Ass1_Product.Models.DTO;
using QE180082_Ass1_Product.Models.Entities;
using QE180082_Ass1_Product.Repositories;

namespace QE180082_Ass1_Product.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            try
            {
                // Check if user exists
                if (await _userRepository.ExistsAsync(request.Email))
                {
                    throw new Exception("Email already registered");
                }

                // Hash password
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

                // Create user
                var user = new UserEntity
                {
                    Email = request.Email,
                    Password = passwordHash,
                    //FullName = request.FullName,
                    //Phone = request.Phone,
                    //Role = "user"
                };

                user = await _userRepository.CreateAsync(user);

                // Generate token
                var token = GenerateJwtToken(user);

                return new AuthResponse
                {
                    UserId = user.Id,
                    Email = user.Email,
                    //FullName = user.FullName,
                    //Role = user.Role,
                    Token = token
                };
            }
            catch (Exception e)
            {
                return null!;
            }
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            // Find user
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null || user.Password == null)
            {
                throw new Exception("Invalid email or password");
            }

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                throw new Exception("Invalid email or password");
            }

            // Generate token
            var token = GenerateJwtToken(user);

            return new AuthResponse
            {
                UserId = user.Id,
                Email = user.Email,
                //FullName = user.FullName,
                //Role = user.Role,
                Token = token
            };
        }

        public async Task<UserEntity?> GetUserByIdAsync(int userId)
        {
            return await _userRepository.GetByIdAsync(userId);
        }

        public async Task<UserEntity> UpdateProfileAsync(int userId, UpdateProfileRequest request)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            //user.FullName = request.FullName ?? user.FullName;
            //user.Phone = request.Phone ?? user.Phone;
            //user.Address = request.Address ?? user.Address;

            return await _userRepository.UpdateAsync(user);
        }

        public string GenerateJwtToken(UserEntity user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "YourSuperSecretKeyForJWTTokenGeneration12345"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
               // new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"] ?? "YourIssuer",
                audience: _configuration["Jwt:Audience"] ?? "YourAudience",
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
