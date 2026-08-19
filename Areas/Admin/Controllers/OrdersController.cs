using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tagr.Exceptions;
using tagr.Services;
using tagr.Services.Interfaces;
using tagr.ViewModels;

namespace tagr.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var orders = await _orderService.GetAllAsync();
            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

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
        public async Task<IActionResult> UpdateStatus(OrderStatusUpdateViewModel model)
        {
            try
            {
                await _orderService.UpdateStatusAsync(model);
                TempData["SuccessMessage"] = "Order status updated successfully.";
            }
            catch (NotFoundException)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Details), new { id = model.Id });
        }
    }
}