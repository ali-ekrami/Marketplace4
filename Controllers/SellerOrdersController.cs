using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using tagr.Exceptions;
using tagr.Models;
using tagr.Services.Interfaces;
using tagr.ViewModels;

namespace tagr.Controllers
{
    // Orders that contain at least one of the current seller's products.
    [Authorize(Roles = "Seller,Admin")]
    public class SellerOrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly UserManager<ApplicationUser> _userManager;

        public SellerOrdersController(IOrderService orderService, UserManager<ApplicationUser> userManager)
        {
            _orderService = orderService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var sellerId = _userManager.GetUserId(User)!;
            var orders = await _orderService.GetBySellerIdAsync(sellerId);
            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var sellerId = _userManager.GetUserId(User)!;

            try
            {
                var order = await _orderService.GetDetailsForSellerAsync(id.Value, sellerId);
                return View(order);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (BusinessRuleException)
            {
                // The order exists but holds none of this seller's products.
                return Forbid();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(OrderStatusUpdateViewModel model)
        {
            var sellerId = _userManager.GetUserId(User)!;

            try
            {
                await _orderService.UpdateStatusBySellerAsync(model, sellerId);
                TempData["SuccessMessage"] = "Order status updated successfully.";
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (BusinessRuleException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Details), new { id = model.Id });
        }
    }
}
