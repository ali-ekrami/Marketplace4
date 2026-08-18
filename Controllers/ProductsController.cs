using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using tagr.Exceptions;
using tagr.Models;
using tagr.Services;
using tagr.Services.Interfaces;
using tagr.ViewModels;

namespace tagr.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProductsController(IProductService productService, UserManager<ApplicationUser> userManager)
        {
            _productService = productService;
            _userManager = userManager;
        }

        // GET: Products (متاحة للكل)
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index(string? searchString, int? categoryId, string? sortOrder)
        {
            var products = await _productService.SearchAsync(searchString, categoryId, sortOrder);

            var categories = await _productService.GetAvailableCategoriesAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", categoryId);

            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentCategory"] = categoryId;
            ViewData["CurrentSort"] = sortOrder;

            return View(products);
        }

        // GET: Products/Details/5 (متاحة للكل)
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var product = await _productService.GetDetailsAsync(id.Value);
                return View(product);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        // GET: Products/MyProducts (منتجات التاجر الحالي بس)
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> MyProducts()
        {
            var sellerId = _userManager.GetUserId(User)!;
            var products = await _productService.GetBySellerIdAsync(sellerId);
            return View(products);
        }

        // GET: Products/Create
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = await _productService.GetCreateFormAsync();
            return View(model);
        }

        // POST: Products/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await RepopulateCategoriesAsync(model);
                return View(model);
            }

            var sellerId = _userManager.GetUserId(User)!;

            try
            {
                await _productService.CreateAsync(model, sellerId);
                TempData["SuccessMessage"] = "Product created successfully.";
                return RedirectToAction(nameof(MyProducts));
            }
            catch (DuplicateEntityException ex)
            {
                ModelState.AddModelError(ex.FieldName, ex.Message);
                await RepopulateCategoriesAsync(model);
                return View(model);
            }
            catch (NotFoundException)
            {
                ModelState.AddModelError(nameof(model.CategoryId), "Selected category does not exist.");
                await RepopulateCategoriesAsync(model);
                return View(model);
            }
        }

        // GET: Products/Edit/5
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ownershipResult = await CheckOwnershipAsync(id.Value);
            if (ownershipResult != null)
            {
                return ownershipResult;
            }

            try
            {
                var model = await _productService.GetForEditAsync(id.Value);
                return View(model);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        // POST: Products/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductEditViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            var ownershipResult = await CheckOwnershipAsync(id);
            if (ownershipResult != null)
            {
                return ownershipResult;
            }

            if (!ModelState.IsValid)
            {
                await RepopulateCategoriesAsync(model);
                return View(model);
            }

            try
            {
                await _productService.UpdateAsync(id, model);
                TempData["SuccessMessage"] = "Product updated successfully.";
                return RedirectToAction(nameof(MyProducts));
            }
            catch (DuplicateEntityException ex)
            {
                ModelState.AddModelError(ex.FieldName, ex.Message);
                await RepopulateCategoriesAsync(model);
                return View(model);
            }
            catch (NotFoundException)
            {
                ModelState.AddModelError(nameof(model.CategoryId), "Selected category does not exist.");
                await RepopulateCategoriesAsync(model);
                return View(model);
            }
        }

        // GET: Products/Delete/5
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ownershipResult = await CheckOwnershipAsync(id.Value);
            if (ownershipResult != null)
            {
                return ownershipResult;
            }

            try
            {
                var model = await _productService.GetDetailsAsync(id.Value);
                return View(model);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        // POST: Products/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ownershipResult = await CheckOwnershipAsync(id);
            if (ownershipResult != null)
            {
                return ownershipResult;
            }

            try
            {
                await _productService.DeleteAsync(id);
                TempData["SuccessMessage"] = "Product deleted successfully.";
            }
            catch (NotFoundException)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(MyProducts));
        }

        // POST: Products/UpdateStock
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStock(int id, int stockQuantity)
        {
            var ownershipResult = await CheckOwnershipAsync(id);
            if (ownershipResult != null)
            {
                return ownershipResult;
            }

            try
            {
                await _productService.UpdateStockAsync(id, stockQuantity);
                TempData["SuccessMessage"] = "Stock updated successfully.";
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (BusinessRuleException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(MyProducts));
        }

        // ===== Helpers =====

        private async Task<IActionResult?> CheckOwnershipAsync(int productId)
        {
            string ownerId;

            try
            {
                ownerId = await _productService.GetOwnerIdAsync(productId);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }

            var currentUserId = _userManager.GetUserId(User);

            if (!User.IsInRole("Admin") && ownerId != currentUserId)
            {
                return Forbid();
            }

            return null;
        }

        private async Task RepopulateCategoriesAsync(ProductCreateViewModel model)
        {
            var form = await _productService.GetCreateFormAsync();
            model.AvailableCategories = form.AvailableCategories;
        }

        private async Task RepopulateCategoriesAsync(ProductEditViewModel model)
        {
            var form = await _productService.GetCreateFormAsync();
            model.AvailableCategories = form.AvailableCategories;
        }
    }
}