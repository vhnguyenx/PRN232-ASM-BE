using QE180082_Ass1_Product.Models.Entities;

namespace QE180082_Ass1_Product.Repositories
{
    public interface IUserRepository
    {
        Task<UserEntity?> GetByIdAsync(int id);
        Task<UserEntity?> GetByEmailAsync(string email);
    //    Task<UserEntity?> GetBySupabaseUserIdAsync(string supabaseUserId);
        Task<UserEntity> CreateAsync(UserEntity user);
        Task<UserEntity> UpdateAsync(UserEntity user);
        Task<bool> ExistsAsync(string email);
    }
}
