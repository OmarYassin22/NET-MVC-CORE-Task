using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ARIBApp.Services.Interfaces.Services;
using ARIBApp.DTOs;

public class DeleteModel : PageModel
{
    private readonly IEmployeeService _employeeService;

    public DeleteModel(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [BindProperty]
    public EmployeeDTO Employee { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var emp = await _employeeService.GetByIdAsync(id);
        if (emp == null)
            return NotFound();

        Employee = emp;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        await _employeeService.DeleteAsync(id);
        return RedirectToPage("Index");
    }
}
