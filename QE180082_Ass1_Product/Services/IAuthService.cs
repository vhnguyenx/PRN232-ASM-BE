using QE180082_Ass1_Product.Models.DTO;
using QE180082_Ass1_Product.Models.Entities;

namespace QE180082_Ass1_Product.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<UserEntity?> GetUserByIdAsync(int userId);
        Task<UserEntity> UpdateProfileAsync(int userId, UpdateProfileRequest request);
        string GenerateJwtToken(UserEntity user);
    }
}
