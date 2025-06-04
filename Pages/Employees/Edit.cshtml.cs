using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ARIBApp.Services.Interfaces.Services;
using ARIBApp.DTOs;

public class EditModel : PageModel
{
    private readonly IEmployeeService _employeeService;

    public EditModel(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [BindProperty]
    public EmployeeDTO Employee { get; set; } = new();

    [BindProperty]
    public IFormFile? ImageFile { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var emp = await _employeeService.GetByIdAsync(id);
        if (emp == null)
            return NotFound();

        Employee = emp;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        // Handle image upload if needed
        if (ImageFile != null)
        {
            // Save image and set Employee.ImageUrl (implement as needed)
        }

        await _employeeService.UpdateAsync(Employee);
        return RedirectToPage("Index");
    }
}
