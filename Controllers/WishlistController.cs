using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tagr.Exceptions;
using tagr.Services.Interfaces;

namespace tagr.Controllers
{
    [Authorize]
    public class WishlistController : Controller
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var items = await _wishlistService.GetAsync();
            return View(items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int productId)
        {
            try
            {
                await _wishlistService.AddAsync(productId);
                TempData["SuccessMessage"] = "Product added to your wishlist.";
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (BusinessRuleException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectBack();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int productId)
        {
            await _wishlistService.RemoveAsync(productId);
            TempData["SuccessMessage"] = "Product removed from your wishlist.";

            return RedirectBack();
        }

        // Keep the user on the page they toggled from (product page or the wishlist itself).
        private IActionResult RedirectBack()
        {
            var referer = Request.Headers.Referer.ToString();

            if (!string.IsNullOrEmpty(referer))
            {
                return Redirect(referer);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
