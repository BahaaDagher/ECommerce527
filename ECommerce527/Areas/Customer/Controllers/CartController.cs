using ECommerce527.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce527.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IRepository<Cart> _cartRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(IRepository<Cart> cartRepository, UserManager<ApplicationUser> userManager, IRepository<Product> productRepository)
        {
            _cartRepository = cartRepository;
            _userManager = userManager;
            _productRepository = productRepository;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();
            var carts = await _cartRepository.GetAsync(c=>c.ApplicationUserId == user.Id , includes: [c=> c.Product]);
            return View(carts);
        }
        public async Task<IActionResult> AddToCart(int count  , int productId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();
            var product = await _productRepository.GetOneAsync(p=>p.Id == productId);
            if (product is null ) return NotFound();
            var cartInDb = await _cartRepository.GetOneAsync(c => c.ProductId == productId && c.ApplicationUserId == user.Id);  
            if (cartInDb != null) {
                cartInDb.Count += count;
                await _cartRepository.CommitAsync();
                return RedirectToAction(nameof(Index));
            }
                var cart = new Cart();
            cart.ApplicationUserId = user.Id;
            cart.ProductId = productId;
            cart.Price = product.Price - ( product.Price * (product.Discount / 100) ) ;
            cart.Count = count; 
            await _cartRepository.AddAsync(cart);
            await _cartRepository.CommitAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> IncrementCount(int productId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();
            var cartInDb = await _cartRepository.GetOneAsync(c => c.ProductId == productId && c.ApplicationUserId == user.Id);
            if (cartInDb is null) return NotFound();
            var product = await _productRepository.GetOneAsync(p => p.Id == productId); 
            if (product.Quantity > cartInDb.Count )
            {
                cartInDb.Count++;
               await _cartRepository.CommitAsync(); 
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> DecrementCount(int productId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();
            var cartInDb = await _cartRepository.GetOneAsync(c => c.ProductId == productId && c.ApplicationUserId == user.Id);
            if (cartInDb is null) return NotFound();
            var product = await _productRepository.GetOneAsync(p => p.Id == productId);
            if (cartInDb.Count > 1)
            {
                cartInDb.Count--;
                await _cartRepository.CommitAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();
            var cartInDb = await _cartRepository.GetOneAsync(c => c.ProductId == productId && c.ApplicationUserId == user.Id);
            if (cartInDb is null) return NotFound();
            _cartRepository.Delete(cartInDb);
            await _cartRepository.CommitAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
