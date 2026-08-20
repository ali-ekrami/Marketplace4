using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using tagr.Exceptions;
using tagr.Models;
using tagr.Services;
using tagr.Services.Interfaces;
using tagr.ViewModels;

namespace tagr.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(ICartService cartService, IOrderService orderService, UserManager<ApplicationUser> userManager)
        {
            _cartService = cartService;
            _orderService = orderService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var cart = await _cartService.GetCartAsync();
            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int productId, int quantity = 1)
        {
            try
            {
                await _cartService.AddToCartAsync(productId, quantity);
                TempData["SuccessMessage"] = "Product added to cart.";
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (BusinessRuleException ex)
            {
                TempData["ErrorMessage"] = ex.Message;

                // On failure (e.g. out of stock), send the user back to where they were.
                var referer = Request.Headers.Referer.ToString();
                if (!string.IsNullOrEmpty(referer))
                {
                    return Redirect(referer);
                }
            }

            // On success, always go to the cart page.
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int productId, int quantity)
        {
            try
            {
                await _cartService.UpdateQuantityAsync(productId, quantity);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (BusinessRuleException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int productId)
        {
            await _cartService.RemoveFromCartAsync(productId);
            TempData["SuccessMessage"] = "Item removed from cart.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var cart = await _cartService.GetCartAsync();

            if (!cart.Items.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.GetUserAsync(User);

            var model = new CheckoutViewModel
            {
                FullName = user?.FullName ?? string.Empty,
                Email = user?.Email ?? string.Empty,
                Items = cart.Items,
                Total = cart.Total
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            var cart = await _cartService.GetCartAsync();

            if (!cart.Items.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                // Re-populate the read-only/display fields since they aren't posted back.
                var user = await _userManager.GetUserAsync(User);
                model.FullName = user?.FullName ?? string.Empty;
                model.Email = user?.Email ?? string.Empty;
                model.Items = cart.Items;
                model.Total = cart.Total;
                return View(model);
            }

            var orderModel = new OrderCreateViewModel
            {
                PhoneNumber = model.PhoneNumber,
                ShippingAddress = $"{model.Address}, {model.City}",
                Items = cart.Items
                    .Select(i => new OrderItemCreateViewModel { ProductId = i.ProductId, Quantity = i.Quantity })
                    .ToList()
            };

            var customerId = _userManager.GetUserId(User)!;

            try
            {
                var orderId = await _orderService.CreateAsync(orderModel, customerId);
                await _cartService.ClearCartAsync();

                TempData["SuccessMessage"] = "Order placed successfully.";
                return RedirectToAction("Details", "Orders", new { id = orderId });
            }
            catch (NotFoundException)
            {
                TempData["ErrorMessage"] = "One of the items in your cart is no longer available.";
                return RedirectToAction(nameof(Index));
            }
            catch (BusinessRuleException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}