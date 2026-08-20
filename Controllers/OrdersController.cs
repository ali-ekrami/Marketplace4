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
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly UserManager<ApplicationUser> _userManager;
        public OrdersController(IOrderService orderService, UserManager<ApplicationUser> userManager)
        {
            _orderService = orderService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var customerId = _userManager.GetUserId(User)!;
            var orders = await _orderService.GetByCustomerIdAsync(customerId);
            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var ownershipResult = await CheckOwnershipAsync(id.Value);
            if (ownershipResult != null) return ownershipResult;

            try
            {
                var order = await _orderService.GetDetailsAsync(id.Value);
                return View(order);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please add at least one valid item to your order.";
                return RedirectToAction(nameof(Index), "Products");
            }

            var customerId = _userManager.GetUserId(User) ?? throw new InvalidOperationException("User not found");

            try
            {
                var orderId = await _orderService.CreateAsync(model, customerId);
                TempData["SuccessMessage"] = "Order created successfully!";
                return RedirectToAction(nameof(Details), new { id = orderId });

            }
            catch (NotFoundException)
            {
                TempData["ErrorMessage"] = "One of the selected products no longer exists.";
                return RedirectToAction(nameof(Index), "Products");
            }
            catch (BusinessRuleException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "Products");
            }
        }

        private async Task<IActionResult?> CheckOwnershipAsync(int orderId)
        {
            string ownerId;

            try
            {
                ownerId = await _orderService.GetOwnerIdAsync(orderId);

            }
            catch (NotFoundException)
            {
                return NotFound();
            }

            var currentUserId = _userManager.GetUserId(User);

            if (!User.IsInRole("Admin") && ownerId != currentUserId)
                return Forbid();

            return null;
        }
    }
}