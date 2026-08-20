using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tagr.Exceptions;
using tagr.Services;
using tagr.Services.Interfaces;

namespace tagr.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SellersController : Controller
    {
        private readonly ISellerService _sellerService;

        public SellersController(ISellerService sellerService)
        {
            _sellerService = sellerService;
        }

        [HttpGet]
        public async Task<IActionResult> Requests()
        {
            var requests = await _sellerService.GetPendingRequestsAsync();
            return View(requests);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(string id)
        {
            try
            {
                await _sellerService.ApproveAsync(id);
                TempData["SuccessMessage"] = "Seller request approved successfully.";
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (BusinessRuleException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Requests));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(string id)
        {
            try
            {
                await _sellerService.RejectAsync(id);
                TempData["SuccessMessage"] = "Seller request rejected.";
            }
            catch (NotFoundException)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Requests));
        }
    }
}