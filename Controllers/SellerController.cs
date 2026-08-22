using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using tagr.Exceptions;
using tagr.Models;
using tagr.Services.Interfaces;

namespace tagr.Controllers
{
    [Authorize]
    public class SellerController : Controller
    {
        private readonly ISellerService _sellerService;
        private readonly UserManager<ApplicationUser> _userManager;

        public SellerController(ISellerService sellerService, UserManager<ApplicationUser> userManager)
        {
            _sellerService = sellerService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Become()
        {
            var user = await _userManager.GetUserAsync(User);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitRequest()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            try
            {
                await _sellerService.RequestAsync(user.Id);
                TempData["SuccessMessage"] = "Your request to become a seller has been submitted.";
            }
            catch (BusinessRuleException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (NotFoundException)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Become));
        }
    }
}