using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tagr.Data;
using tagr.Models;

namespace tagr.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Categories
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
                .AsNoTracking()
                .Include(c => c.Products)
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View(categories);
        }

        // GET: Categories/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            category.Name = category.Name?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(category.Name))
            {
                ModelState.AddModelError(
                    nameof(category.Name),
                    "Category name is required.");
            }

            if (category.Name.Length > 100)
            {
                ModelState.AddModelError(
                    nameof(category.Name),
                    "Category name cannot exceed 100 characters.");
            }

            var exists = await _context.Categories
                .AnyAsync(c => c.Name.ToLower() == category.Name.ToLower());

            if (exists)
            {
                ModelState.AddModelError(
                    nameof(category.Name),
                    "A category with this name already exists.");
            }

            if (!ModelState.IsValid)
            {
                return View(category);
            }

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Category created successfully.";

            return RedirectToAction(nameof(Index));
        }

        // GET: Categories/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id.Value);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Name")]
            Category category)
        {
            if (id != category.Id)
            {
                return BadRequest();
            }

            category.Name = category.Name?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(category.Name))
            {
                ModelState.AddModelError(
                    nameof(category.Name),
                    "Category name is required.");
            }

            if (category.Name.Length > 100)
            {
                ModelState.AddModelError(
                    nameof(category.Name),
                    "Category name cannot exceed 100 characters.");
            }

            var duplicateExists = await _context.Categories
                .AnyAsync(c =>
                    c.Id != id &&
                    c.Name.ToLower() == category.Name.ToLower());

            if (duplicateExists)
            {
                ModelState.AddModelError(
                    nameof(category.Name),
                    "A category with this name already exists.");
            }

            if (!ModelState.IsValid)
            {
                return View(category);
            }

            var existingCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id);

            if (existingCategory == null)
            {
                return NotFound();
            }

            existingCategory.Name = category.Name;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Category updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        // GET: Categories/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .AsNoTracking()
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id.Value);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            // Do not delete a category that still contains products.
            // This prevents accidental deletion of products.
            if (category.Products.Any())
            {
                TempData["ErrorMessage"] =
                    "This category cannot be deleted because it contains products.";

                return RedirectToAction(nameof(Index));
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Category deleted successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}
