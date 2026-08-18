using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tagr.Exceptions;
using tagr.Services.Interfaces;
using tagr.ViewModels;

namespace tagr.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllAsync();
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create() => View(new CategoryCreateViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                await _categoryService.CreateAsync(model);
                TempData["SuccessMessage"] = "Category created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DuplicateEntityException ex)
            {
                ModelState.AddModelError(ex.FieldName, ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var model = await _categoryService.GetForEditAsync(id.Value);
                return View(model);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryEditViewModel model)
        {
            if (id != model.Id) return BadRequest();

            if (!ModelState.IsValid) return View(model);

            try
            {
                await _categoryService.UpdateAsync(id, model);
                TempData["SuccessMessage"] = "Category updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DuplicateEntityException ex)
            {
                ModelState.AddModelError(ex.FieldName, ex.Message);
                return View(model);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var model = await _categoryService.GetForDeleteAsync(id.Value);
                return View(model);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _categoryService.DeleteAsync(id);
                TempData["SuccessMessage"] = "Category deleted successfully.";
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
    }
}