using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ARIBApp.Services.Interfaces.Services;
using ARIBApp.DTOs;

public class CreateModel : PageModel
{
    private readonly IEmployeeService _employeeService;

    public CreateModel(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [BindProperty]
    public EmployeeDTO Employee { get; set; } = new();

    [BindProperty]
    public IFormFile? ImageFile { get; set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        // Handle image upload if needed
        if (ImageFile != null)
        {
            // Save image and set Employee.ImageUrl (implement as needed)
        }

        await _employeeService.AddAsync(Employee);
        return RedirectToPage("Index");
    }
}
