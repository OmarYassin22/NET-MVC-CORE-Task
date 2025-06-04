using Microsoft.AspNetCore.Mvc.RazorPages;
using ARIBApp.Services.Interfaces.Services;
using ARIBApp.DTOs;
using Microsoft.AspNetCore.Mvc;

public class IndexModel : PageModel
{
    private readonly IEmployeeService _employeeService;

    public IndexModel(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    public IReadOnlyList<EmployeeDTO> Employees { get; set; } = [];

    public async Task OnGetAsync()
    {
        Employees = await _employeeService.GetAllAsync();
    }

    public async Task<IActionResult> OnGetSearchAsync(string keyword)
    {
        var emps = await _employeeService.SearchAsync(keyword ?? "");
        return new JsonResult(emps);
    }
}
