using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using tagr.Exceptions;
using tagr.Models;
using tagr.Services.Interfaces;
using tagr.ViewModels;

namespace tagr.Controllers
{
    [Authorize]
    public class ReviewsController : Controller
    {
        private readonly IReviewService _reviewService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewsController(IReviewService reviewService, UserManager<ApplicationUser> userManager)
        {
            _reviewService = reviewService;
            _userManager = userManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReviewCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please choose a rating between 1 and 5 and keep the comment under 1000 characters.";
                return RedirectToProduct(model.ProductId);
            }

            var customerId = _userManager.GetUserId(User)!;

            try
            {
                await _reviewService.CreateAsync(model, customerId);
                TempData["SuccessMessage"] = "Thanks for your review.";
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (BusinessRuleException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToProduct(model.ProductId);
        }

        private IActionResult RedirectToProduct(int productId)
            => RedirectToAction("Details", "Products", new { id = productId });
    }
}
