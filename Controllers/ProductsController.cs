using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using tagr.Data;
using tagr.Models;

namespace tagr.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // CUSTOMER / PUBLIC
        // Browse + Search + Filter + Sort
        // =========================================================
        [AllowAnonymous]
        public async Task<IActionResult> Index(
            string? searchString,
            int? categoryId,
            string? sortOrder)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentCategory"] = categoryId;
            ViewData["CurrentSort"] = sortOrder;

            var products = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Seller)
                .AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                searchString = searchString.Trim();

                products = products.Where(p =>
                    p.Name.Contains(searchString) ||
                    p.Description.Contains(searchString));
            }

            // Category Filter
            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value);
            }

            // Sorting
            products = sortOrder switch
            {
                "name_desc" => products.OrderByDescending(p => p.Name),
                "price_asc" => products.OrderBy(p => p.Price),
                "price_desc" => products.OrderByDescending(p => p.Price),
                _ => products.OrderBy(p => p.Name)
            };

            ViewBag.Categories = new SelectList(
                await _context.Categories
                    .OrderBy(c => c.Name)
                    .ToListAsync(),
                "Id",
                "Name",
                categoryId);

            return View(await products.ToListAsync());
        }

        // =========================================================
        // CUSTOMER / PUBLIC
        // Product Details
        // =========================================================
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Seller)
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // =========================================================
        // SELLER
        // My Products
        // =========================================================
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> MyProducts()
        {
            var sellerId = GetCurrentUserId();

            if (sellerId == null)
            {
                return Challenge();
            }

            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.SellerId == sellerId)
                .OrderBy(p => p.Name)
                .ToListAsync();

            return View(products);
        }
        // =========================================================
        // SELLER
        // Create - GET
        // =========================================================
        [Authorize(Roles = "Seller")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadCategoriesAsync();

            return View();
        }

        // =========================================================
        // SELLER
        // Create - POST
        // =========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> Create(
    string Name,
    string Description,
    decimal Price,
    int StockQuantity,
    int CategoryId,
    string ImageUrl)
        {
           

            var sellerId = GetCurrentUserId();

            if (sellerId == null)
            {
                return Challenge();
            }

            var categoryExists = await _context.Categories
                .AnyAsync(c => c.Id == CategoryId);

            if (!categoryExists)
            {
                ModelState.AddModelError(
                    "CategoryId",
                    "Please select a valid category.");
            }

            if (Price < 0)
            {
                ModelState.AddModelError(
                    "Price",
                    "Price cannot be negative.");
            }

            if (StockQuantity < 0)
            {
                ModelState.AddModelError(
                    "StockQuantity",
                    "Stock quantity cannot be negative.");
            }

            if (!ModelState.IsValid)
            {
                var product = new Product
                {
                    Name = Name,
                    Description = Description,
                    Price = Price,
                    StockQuantity = StockQuantity,
                    CategoryId = CategoryId,
                    ImageUrl = ImageUrl ?? string.Empty,
                    SellerId = sellerId
                };

                await LoadCategoriesAsync(CategoryId);

                return View(product);
            }

            var newProduct = new Product
            {
                Name = Name,
                Description = Description,
                Price = Price,
                StockQuantity = StockQuantity,
                CategoryId = CategoryId,
                ImageUrl = ImageUrl ?? string.Empty,
                SellerId = sellerId
            };

            _context.Products.Add(newProduct);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Product added successfully.";

            return RedirectToAction(nameof(MyProducts));
        }
        // =========================================================
        // SELLER
        // Edit - GET
        // =========================================================
        [Authorize(Roles = "Seller")]
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
          

            if (id == null)
            {
                return NotFound();
            }

            var sellerId = GetCurrentUserId();

            if (sellerId == null)
            {
                return Challenge();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(p =>
                    p.Id == id.Value &&
                    p.SellerId == sellerId);

            if (product == null)
            {
                return NotFound();
            }

            await LoadCategoriesAsync(product.CategoryId);

            return View(product);
        }

        // =========================================================
        // SELLER
        // Edit - POST
        // =========================================================
        [Authorize(Roles = "Seller")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
           

            if (id != product.Id)
            {
                return NotFound();
            }

            var sellerId = GetCurrentUserId();

            if (sellerId == null)
            {
                return Challenge();
            }
            ModelState.Remove(nameof(Product.Category));
            ModelState.Remove(nameof(Product.Seller));
            ModelState.Remove(nameof(Product.OrderItems));
            ModelState.Remove(nameof(Product.Reviews));
            // IMPORTANT:
            // Find the product using BOTH its Id and current SellerId.
            // This prevents a Seller from editing another Seller's product.
            var existingProduct = await _context.Products
                .FirstOrDefaultAsync(p =>
                    p.Id == id &&
                    p.SellerId == sellerId);

            if (existingProduct == null)
            {
                return NotFound();
            }

            if (!await _context.Categories
                .AnyAsync(c => c.Id == product.CategoryId))
            {
                ModelState.AddModelError(
                    "CategoryId",
                    "Please select a valid category.");
            }

            if (product.Price < 0)
            {
                ModelState.AddModelError(
                    "Price",
                    "Price cannot be negative.");
            }

            if (product.StockQuantity < 0)
            {
                ModelState.AddModelError(
                    "StockQuantity",
                    "Stock quantity cannot be negative.");
            }

            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync(product.CategoryId);
                return View(product);
            }

            // Update only the fields the Seller is allowed to edit.
            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.StockQuantity = product.StockQuantity;
            existingProduct.ImageUrl = product.ImageUrl;
            existingProduct.CategoryId = product.CategoryId;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Product updated successfully.";

            return RedirectToAction(nameof(MyProducts));
        }

        // =========================================================
        // SELLER
        // Delete - GET
        // =========================================================
        [Authorize(Roles = "Seller")]
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            

            if (id == null)
            {
                return NotFound();
            }

            var sellerId = GetCurrentUserId();

            if (sellerId == null)
            {
                return Challenge();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p =>
                    p.Id == id.Value &&
                    p.SellerId == sellerId);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // =========================================================
        // SELLER
        // Delete - POST
        // =========================================================
        [Authorize(Roles = "Seller")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var sellerId = GetCurrentUserId();

            if (sellerId == null)
            {
                return Challenge();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(p =>
                    p.Id == id &&
                    p.SellerId == sellerId);

            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Product deleted successfully.";

            return RedirectToAction(nameof(MyProducts));
        }
        // =========================================================
        // SELLER
        // Update Stock
        // =========================================================
        [Authorize(Roles = "Seller")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStock(
            int id,
            int stockQuantity)
        {
           

            if (stockQuantity < 0)
            {
                TempData["ErrorMessage"] =
                    "Stock quantity cannot be negative.";

                return RedirectToAction(nameof(MyProducts));
            }

            var sellerId = GetCurrentUserId();

            if (sellerId == null)
            {
                return Challenge();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(p =>
                    p.Id == id &&
                    p.SellerId == sellerId);

            if (product == null)
            {
                return NotFound();
            }

            product.StockQuantity = stockQuantity;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Stock quantity updated successfully.";

            return RedirectToAction(nameof(MyProducts));
        }
        
        // =========================================================
        // Helper Methods
        // =========================================================

        private string? GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        private async Task<bool> IsApprovedSellerAsync()
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return false;
            }

            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            return user != null &&
                   user.IsSellerApproved &&
                   !user.IsSuspended;
        }

        private async Task LoadCategoriesAsync(int? selectedCategoryId = null)
        {
            var categories = await _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();

            ViewBag.Categories = new SelectList(
                categories,
                "Id",
                "Name",
                selectedCategoryId);
        }
    }
}