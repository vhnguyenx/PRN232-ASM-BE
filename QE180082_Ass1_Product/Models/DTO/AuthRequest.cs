namespace QE180082_Ass1_Product.Models.DTO
{
    public class RegisterRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
      //  public string? FullName { get; set; }
      //  public string? Phone { get; set; }
    }

    public class LoginRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class AuthResponse
    {
        public int UserId { get; set; }
        public required string Email { get; set; }
        public string? FullName { get; set; }
        public string? Role { get; set; }
        public required string Token { get; set; }
    }

    public class UpdateProfileRequest
    {
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }
}
