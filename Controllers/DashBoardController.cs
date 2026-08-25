using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using tagr.Models;
using tagr.Services.Interfaces;

namespace tagr.Controllers
{
    [Authorize]
    public class DashBoardController : Controller
    {
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashBoardController(
            IUserService userService,
            IProductService productService,
            IOrderService orderService,
            UserManager<ApplicationUser> userManager)
        {
            _userService = userService;
            _productService = productService;
            _orderService = orderService;
            _userManager = userManager;
        }

        // =========================
        // Admin Dashboard
        // =========================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllAsync();
            var products = await _productService.GetAllAsync();
            var orders = await _orderService.GetAllAsync();

            var sellers = await _userManager.GetUsersInRoleAsync("Seller");
            var admins = await _userManager.GetUsersInRoleAsync("Admin");

            ViewBag.TotalCustomers = users.Count(u =>
                !sellers.Any(s => s.Id == u.Id) &&
                !admins.Any(a => a.Id == u.Id));

            ViewBag.TotalSellers = sellers.Count;

            ViewBag.TotalProducts = products.Count();

            ViewBag.TotalOrders = orders.Count();

            ViewBag.PendingOrders = orders.Count(o =>
                o.Status == OrderStatus.Pending);

            return View();
        }

        // =========================
        // Seller Dashboard
[Authorize(Roles = "Seller")]
public async Task<IActionResult> Seller()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
                return Unauthorized();

            // Seller's products
            var myProducts =
                await _productService.GetBySellerIdAsync(currentUser.Id);

            ViewBag.TotalProducts = myProducts.Count;

            // Seller's orders
            var myOrders =
                await _orderService.GetBySellerIdAsync(currentUser.Id);

            // Total orders
            ViewBag.TotalOrders = myOrders.Count;

            // Pending orders
            ViewBag.PendingOrders = myOrders.Count(o =>
                o.Status == OrderStatus.Pending);

            // Total sales for this seller only
            ViewBag.TotalSales = myOrders
                .Where(o => o.Status != OrderStatus.Cancelled)
                .Sum(o => o.MyItemsTotal);

            return View();
        }

    }
}