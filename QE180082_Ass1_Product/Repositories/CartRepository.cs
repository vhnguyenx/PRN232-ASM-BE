using Microsoft.EntityFrameworkCore;
using QE180082_Ass1_Product.Models.Entities;

namespace QE180082_Ass1_Product.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _context;

        public CartRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CartItemEntity>> GetCartItemsByUserIdAsync(int userId)
        {
            return await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task<CartItemEntity?> GetCartItemAsync(int userId, int productId)
        {
            return await _context.CartItems
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);
        }

        public async Task<CartItemEntity?> GetCartItemByIdAsync(int id)
        {
            return await _context.CartItems
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<CartItemEntity> AddCartItemAsync(CartItemEntity cartItem)
        {
            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();
            return cartItem;
        }

        public async Task<CartItemEntity> UpdateCartItemAsync(CartItemEntity cartItem)
        {
         //   cartItem.UpdatedAt = DateTime.UtcNow;
            _context.CartItems.Update(cartItem);
            await _context.SaveChangesAsync();
            return cartItem;
        }

        public async Task DeleteCartItemAsync(int id)
        {
            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync(int userId)
        {
            var cartItems = await _context.CartItems
                .Where(c => c.UserId == userId)
                .ToListAsync();
            
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
        }
    }
}
