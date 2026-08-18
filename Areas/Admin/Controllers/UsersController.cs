using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tagr.Exceptions;
using tagr.Services;
using tagr.Services.Interfaces;

namespace tagr.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: Admin/Users
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllAsync();
            return View(users);
        }

        // POST: Admin/Users/ToggleStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(string id)
        {
            try
            {
                await _userService.ToggleStatusAsync(id);
                TempData["SuccessMessage"] = "User status updated successfully.";
            }
            catch (NotFoundException)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}