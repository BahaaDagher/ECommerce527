using ECommerce527.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace ECommerce527.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IRepository<Cart> _cartRepository;
        private readonly IRepository<Promotion> _promotionRepository;
        private readonly IRepository<Models.Product> _productRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(IRepository<Cart> cartRepository, UserManager<ApplicationUser> userManager, IRepository<Models.Product> productRepository, IRepository<Promotion> promotionRepository)
        {
            _cartRepository = cartRepository;
            _userManager = userManager;
            _productRepository = productRepository;
            _promotionRepository = promotionRepository;
        }

        public async Task<IActionResult> Index(string code)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();
            if(code != null)
            {
                var promotion = await _promotionRepository.GetOneAsync(p=>
                    p.Code == code && 
                    p.IsValid && 
                    DateTime.UtcNow <  p.ValidTo &&
                    p.MaxUsage > 0
                );
                if(promotion != null)
                {
                    var cart = await _cartRepository.GetOneAsync(c=>
                        c.ApplicationUserId == user.Id &&
                        c.ProductId == promotion.ProductId
                    ); 
                    if(cart != null)
                    {
                        cart.Price -= promotion.Discount / 100;
                        promotion.MaxUsage--; 
                        await _cartRepository.CommitAsync(); 
                        await _promotionRepository.CommitAsync();
                        TempData["Success-Notification"] = "promotion Applied Successfully ";
                    }
                    else
                    {
                        TempData["Error-Notification"] = "there is no product here can apply this promotion on it ";
                    }
                }
                else
                {
                    TempData["Error-Notification"] = "invalid / Expired Promotion";
                }
            }
            var carts = await _cartRepository.GetAsync(c=>c.ApplicationUserId == user.Id , includes: [c=> c.Product]);
            var TotalPrice = carts.Sum(c=>c.Price* c.Count);
            ViewBag.TotalPrice = TotalPrice;
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
        public async Task<IActionResult> Pay()
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/checkout/success",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/checkout/cancel",
            };
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();
            var carts = await _cartRepository.GetAsync(c=>c.ApplicationUserId == user.Id , includes: [p=>p.Product]);
            if (carts is null) return NotFound();
            foreach(var cart in carts)
            {
                var sessionLineItemOptions = new SessionLineItemOptions()
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "egp",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = cart.Product.Name,
                            Description = cart.Product.Description,
                        },
                        UnitAmount = (long) cart.Price * 100,
                    },
                    Quantity = cart.Count,
                }; 
                options.LineItems.Add(sessionLineItemOptions); 
            }
            var service = new SessionService();
            var session = service.Create(options);
            return Redirect(session.Url);
        }
    }
}
