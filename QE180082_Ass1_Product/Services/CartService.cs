using QE180082_Ass1_Product.Models.DTO;
using QE180082_Ass1_Product.Models.Entities;
using QE180082_Ass1_Product.Repositories;

namespace QE180082_Ass1_Product.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;

        public CartService(ICartRepository cartRepository, IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        public async Task<CartResponse> GetCartAsync(int userId)
        {
            var cartItems = await _cartRepository.GetCartItemsByUserIdAsync(userId);

            var items = cartItems.Select(c => new CartItemResponse
            {
                Id = c.Id,
                ProductId = c.ProductId,
                ProductName = c.Product.Name,
                ProductImage = c.Product.Image,
                ProductPrice = c.Product.Price,
                Quantity = c.Quantity,
                Subtotal = c.Product.Price * c.Quantity,
              //  Stock = c.Product.Stock
            }).ToList();

            return new CartResponse
            {
                Items = items,
                TotalAmount = items.Sum(i => i.Subtotal),
                TotalItems = items.Sum(i => i.Quantity)
            };
        }

        public async Task<CartItemResponse> AddToCartAsync(int userId, AddToCartRequest request)
        {
            // Check if product exists
            var product = await _productRepository.GetByIdAsync(request.ProductId);
            if (product == null)
            {
                throw new Exception("Product not found");
            }

            // Check stock
            //if (product.Stock < request.Quantity)
            //{
            //    throw new Exception("Insufficient stock");
            //}

            // Check if item already in cart
            var existingItem = await _cartRepository.GetCartItemAsync(userId, request.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += request.Quantity;
                existingItem = await _cartRepository.UpdateCartItemAsync(existingItem);

                return new CartItemResponse
                {
                    Id = existingItem.Id,
                    ProductId = existingItem.ProductId,
                    ProductName = product.Name,
                    ProductImage = product.Image,
                    ProductPrice = product.Price,
                    Quantity = existingItem.Quantity,
                    Subtotal = product.Price * existingItem.Quantity,
                 //   Stock = product.Stock
                };
            }

            // Add new item
            var cartItem = new CartItemEntity
            {
                UserId = userId,
                ProductId = request.ProductId,
                Quantity = request.Quantity
            };

            cartItem = await _cartRepository.AddCartItemAsync(cartItem);

            return new CartItemResponse
            {
                Id = cartItem.Id,
                ProductId = cartItem.ProductId,
                ProductName = product.Name,
                ProductImage = product.Image,
                ProductPrice = product.Price,
                Quantity = cartItem.Quantity,
                Subtotal = product.Price * cartItem.Quantity,
               // Stock = product.Stock
            };
        }

        public async Task<CartItemResponse> UpdateCartItemAsync(int userId, int cartItemId, UpdateCartItemRequest request)
        {
            var cartItem = await _cartRepository.GetCartItemByIdAsync(cartItemId);
            if (cartItem == null || cartItem.UserId != userId)
            {
                throw new Exception("Cart item not found");
            }

            var product = await _productRepository.GetByIdAsync(cartItem.ProductId);
            if (product == null)
            {
                throw new Exception("Product not found");
            }

            //// Check stock
            //if (product.Stock < request.Quantity)
            //{
            //    throw new Exception("Insufficient stock");
            //}

            cartItem.Quantity = request.Quantity;

            if (cartItem.UpdatedAt.Kind == DateTimeKind.Unspecified || cartItem.CreatedAt.Kind == DateTimeKind.Unspecified)
            {
                cartItem.UpdatedAt = DateTime.SpecifyKind(cartItem.UpdatedAt, DateTimeKind.Utc);
                cartItem.CreatedAt = DateTime.SpecifyKind(cartItem.CreatedAt, DateTimeKind.Utc);
            }


            cartItem = await _cartRepository.UpdateCartItemAsync(cartItem);

            return new CartItemResponse
            {
                Id = cartItem.Id,
                ProductId = cartItem.ProductId,
                ProductName = product.Name,
                ProductImage = product.Image,
                ProductPrice = product.Price,
                Quantity = cartItem.Quantity,
                Subtotal = product.Price * cartItem.Quantity,
              //  Stock = product.Stock
            };
        }

        public async Task DeleteCartItemAsync(int userId, int cartItemId)
        {
            var cartItem = await _cartRepository.GetCartItemByIdAsync(cartItemId);
            if (cartItem == null || cartItem.UserId != userId)
            {
                throw new Exception("Cart item not found");
            }

            await _cartRepository.DeleteCartItemAsync(cartItemId);
        }

        public async Task ClearCartAsync(int userId)
        {
            await _cartRepository.ClearCartAsync(userId);
        }
    }
}
