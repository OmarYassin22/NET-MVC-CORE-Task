using ARIBApp.Core.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NET_MVC_CORE_Task.Controllers;

[Authorize]
public class DepartmentController : Controller
{
    private readonly IDepartmentService _departmentService;

    public DepartmentController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    private bool IsAjaxRequest()
    {
        return Request.Headers["X-Requested-With"] == "XMLHttpRequest";
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Index(string? searchTerm = "")
    {
        IReadOnlyList<DepartmentDTO> departments;

        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            departments = await _departmentService.GetAllAsync();
        }
        else
        {
            departments = await _departmentService.SearchWithStatsAsync(searchTerm);
        }

        ViewBag.SearchTerm = searchTerm ?? "";

        if (IsAjaxRequest())
        {
            return PartialView("_DepartmentIndex", departments);
        }

        return View(departments);
    }
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Details(int id)
    {
        var department = await _departmentService.GetByIdAsync(id);
        if (department == null)
            return NotFound();

        if (IsAjaxRequest())
        {
            return PartialView("_DepartmentDetails", department);
        }

        return View(department);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        if (IsAjaxRequest())
        {
            return PartialView("_CreateForm", new DepartmentDTO());
        }

        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(DepartmentDTO department)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _departmentService.AddAsync(department);

                if (IsAjaxRequest())
                {
                    return Json(new
                    {
                        success = true,
                        message = $"Department {department.Name} created successfully!",
                        redirectUrl = Url.Action("Index")
                    });
                }

                TempData["SuccessMessage"] = $"Department {department.Name} created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error creating department: {ex.Message}");

                if (IsAjaxRequest())
                {
                    return Json(new
                    {
                        success = false,
                        message = $"Error creating department: {ex.Message}"
                    });
                }

                TempData["ErrorMessage"] = "An error occurred while creating the department.";
            }
        }

        if (IsAjaxRequest())
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return Json(new
            {
                success = false,
                message = "Please correct the errors before saving.",
                errors = errors
            });
        }

        return View(department);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var department = await _departmentService.GetByIdAsync(id);
        if (department == null)
            return NotFound();

        if (IsAjaxRequest())
        {
            return PartialView("_EditForm", department);
        }

        return View(department);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(DepartmentDTO department)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _departmentService.UpdateAsync(department);

                if (IsAjaxRequest())
                {
                    return Json(new
                    {
                        success = true,
                        message = $"Department {department.Name} updated successfully!",
                        redirectUrl = Url.Action("Index")
                    });
                }

                TempData["SuccessMessage"] = $"Department {department.Name} updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error updating department: {ex.Message}");

                if (IsAjaxRequest())
                {
                    return Json(new
                    {
                        success = false,
                        message = $"Error updating department: {ex.Message}"
                    });
                }

                TempData["ErrorMessage"] = "An error occurred while updating the department.";
            }
        }

        if (IsAjaxRequest())
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return Json(new
            {
                success = false,
                message = "Please correct the errors before saving.",
                errors = errors
            });
        }

        return View(department);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var department = await _departmentService.GetByIdAsync(id);
            if (department == null)
            {
                return Json(new { success = false, message = "Department not found." });
            }

            await _departmentService.DeleteAsync(id);
            return Json(new { success = true, message = $"Department {department.Name} deleted successfully." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Search(string keyword)
    {
        var departments = await _departmentService.SearchWithStatsAsync(keyword);

        if (IsAjaxRequest())
        {
            return PartialView("_DepartmentsList", departments);
        }

        return View("Index", departments);
    }
}