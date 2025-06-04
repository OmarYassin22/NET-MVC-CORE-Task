using ARIBApp.Core.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace NET_MVC_CORE_Task.Controllers;

[Authorize]
public class EmployeeController : Controller
{
    private readonly IEmployeeService _employeeService;
    private readonly ITaskService _taskService;
    private readonly IDepartmentService _departmentService;
    private readonly UserManager<Employee> _userManager;
    private readonly IFileService _fileService;

    public EmployeeController(
        IEmployeeService employeeService,
        ITaskService taskService,
        IDepartmentService departmentService,
        UserManager<Employee> userManager,
        IFileService fileService)
    {
        _employeeService = employeeService;
        _taskService = taskService;
        _departmentService = departmentService;
        _userManager = userManager;
        _fileService = fileService;
    }

    private bool IsAjaxRequest()
    {
        return Request.Headers["X-Requested-With"] == "XMLHttpRequest";
    }
    [HttpGet]
    public async Task<IActionResult> Index(string? searchTerm = "")
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        if (User.IsInRole("Admin"))
        {
            var allEmployees = string.IsNullOrWhiteSpace(searchTerm)
               ? await _employeeService.GetAllAsync()
               : await _employeeService.SearchAsync(searchTerm);

            ViewBag.UserRole = "Admin";
            ViewBag.SearchTerm = searchTerm ?? "";
            if (IsAjaxRequest() && !string.IsNullOrWhiteSpace(searchTerm))
            {
                return PartialView("_EmployeeIndex", allEmployees);
            }
            else if (IsAjaxRequest())
            {
                return PartialView("_EmployeeIndex", allEmployees);
            }

            return View("Index", allEmployees);
        }
        else if (User.IsInRole("Manager"))
        {
            return RedirectToAction("ManagerDashboard");
        }
        else
        {

            return RedirectToAction("EmployeeDashboard");
        }
    }
    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Details(int id)
    {
        var employee = await _employeeService.GetByIdAsync(id);
        if (employee == null)
            return NotFound();

        if (User.IsInRole("Manager"))
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (employee.ManagerId != currentUser.Id)
            {
                return Forbid();
            }
        }

        if (IsAjaxRequest())
        {
            return PartialView("_EmployeeDetails", employee);
        }

        return View(employee);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create()
    {
        var departments = await _departmentService.GetAllAsync();
        ViewBag.Departments = departments;

        var managers = await _employeeService.GetAllAsync();
        ViewBag.Managers = managers;

        if (IsAjaxRequest())
        {
            return PartialView("_CreateForm", new EmployeeDTO());
        }

        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(EmployeeDTO dto, string password, IFormFile? imageFile)
    {
        if (ModelState.IsValid)
        {
            try
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    string imagePath = await _fileService.SaveImageAsync(imageFile);
                    dto = dto with { ImagePath = imagePath };
                }
                else
                {
                    dto = dto with { ImagePath = _fileService.GetDefaultImagePath() };
                }

                var employee = new Employee
                {
                    UserName = dto.UserName,
                    Email = dto.Email,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    ImagePath = dto.ImagePath,
                    Salary = dto.Salary,
                    ManagerId = dto.ManagerId,
                    DepartmentId = dto.DepartmentId
                };

                 var result = await _userManager.CreateAsync(employee, password);

                if (result.Succeeded)
                {
                     await _userManager.AddToRoleAsync(employee, "Employee");

                    TempData["SuccessMessage"] = $"Employee {employee.FullName} created successfully!";

                    if (IsAjaxRequest())
                    {
                        return Json(new
                        {
                            success = true,
                            message = $"Employee {employee.FullName} created successfully!",
                            redirectUrl = Url.Action("Index")
                        });
                    }

                    return RedirectToAction("Index");
                }
                else
                {
                     if (!string.IsNullOrEmpty(dto.ImagePath) && dto.ImagePath != _fileService.GetDefaultImagePath())
                    {
                        _fileService.DeleteImage(dto.ImagePath);
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    if (IsAjaxRequest())
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Failed to create employee account.",
                            errors = result.Errors.Select(e => e.Description).ToList()
                        });
                    }

                    TempData["ErrorMessage"] = "Failed to create employee account.";
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error creating employee: {ex.Message}");

                if (IsAjaxRequest())
                {
                    return Json(new
                    {
                        success = false,
                        message = $"Error creating employee: {ex.Message}"
                    });
                }

                TempData["ErrorMessage"] = "An error occurred while creating the employee.";
            }
        }
        else
        {
             var errorMessages = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            if (IsAjaxRequest())
            {
                return Json(new
                {
                    success = false,
                    message = "Please correct the validation errors before saving.",
                    errors = errorMessages
                });
            }

            TempData["ValidationErrors"] = errorMessages;
            TempData["ErrorMessage"] = "Please correct the validation errors before saving.";
        }

         var departments = await _departmentService.GetAllAsync();
        ViewBag.Departments = departments;

        var managers = await _employeeService.GetAllAsync();
        ViewBag.Managers = managers;

        if (IsAjaxRequest())
        {
            return PartialView("_CreateForm", dto);
        }

        return View(dto);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var employee = await _employeeService.GetByIdAsync(id);
        if (employee == null)
            return NotFound();

         var departments = await _departmentService.GetAllAsync();
        ViewBag.Departments = departments;

         var allEmployees = await _employeeService.GetAllAsync();
        var managerId = await _userManager.Users
           .Where(e => e.ManagerId != null)
           .Select(i => i.ManagerId).ToListAsync();
        var potentialManagers = allEmployees
            .Where(e => e.Id != id && managerId.Contains(e.Id)) 
            .ToList();

        ViewBag.Managers = potentialManagers;

        if (IsAjaxRequest())
        {
            return PartialView("_EditForm", employee);
        }

        return View(employee);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(EmployeeDTO dto, string password = null, IFormFile imageFile = null)
    {
        if (ModelState.IsValid)
        {
            try
            {
                 var existingEmployee = await _employeeService.GetByIdAsync(dto.Id);
                if (existingEmployee == null)
                {
                    if (IsAjaxRequest())
                    {
                        return Json(new { success = false, message = "Employee not found" });
                    }

                    return NotFound();
                }

                 if (imageFile != null && imageFile.Length > 0)
                {
                     if (!string.IsNullOrEmpty(existingEmployee.ImagePath) &&
                        existingEmployee.ImagePath != _fileService.GetDefaultImagePath())
                    {
                        _fileService.DeleteImage(existingEmployee.ImagePath);
                    }

                     string imagePath = await _fileService.SaveImageAsync(imageFile);
                    dto = dto with { ImagePath = imagePath };
                }
                else
                {
                     dto = dto with { ImagePath = existingEmployee.ImagePath };
                }

                 await _employeeService.UpdateAsync(dto);

                 if (!string.IsNullOrEmpty(password))
                {
                    var user = await _userManager.FindByIdAsync(dto.Id.ToString());
                    if (user != null)
                    {
                        var removePasswordResult = await _userManager.RemovePasswordAsync(user);
                        if (removePasswordResult.Succeeded)
                        {
                            var addPasswordResult = await _userManager.AddPasswordAsync(user, password);
                            if (!addPasswordResult.Succeeded)
                            {
                                foreach (var error in addPasswordResult.Errors)
                                {
                                    ModelState.AddModelError("", error.Description);
                                }

                                if (IsAjaxRequest())
                                {
                                    return Json(new
                                    {
                                        success = false,
                                        message = "Failed to update password.",
                                        errors = addPasswordResult.Errors.Select(e => e.Description).ToList()
                                    });
                                }

                                TempData["ErrorMessage"] = "Failed to update password.";
                            }
                        }
                    }
                }

                TempData["SuccessMessage"] = $"Employee {dto.FullName} updated successfully!";

                if (IsAjaxRequest())
                {
                    return Json(new
                    {
                        success = true,
                        message = $"Employee {dto.FullName} updated successfully!",
                        redirectUrl = Url.Action("Index")
                    });
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error updating employee: {ex.Message}");

                if (IsAjaxRequest())
                {
                    return Json(new { success = false, message = $"Error updating employee: {ex.Message}" });
                }

                TempData["ErrorMessage"] = "An error occurred while updating the employee.";
            }
        }
        else
        {
            var errorMessages = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            if (IsAjaxRequest())
            {
                return Json(new
                {
                    success = false,
                    message = "Please correct the validation errors before saving.",
                    errors = errorMessages
                });
            }
        }

         var departments = await _departmentService.GetAllAsync();
        ViewBag.Departments = departments;

        var allEmployees = await _employeeService.GetAllAsync();
        var managerId = await _userManager.Users
            .Where(e => e.ManagerId != null)
            .Select(i => i.ManagerId).ToListAsync();
        var potentialManagers = allEmployees
            .Where(e => e.Id != dto.Id && managerId.Contains(e.Id))
            .ToList();

        ViewBag.Managers = potentialManagers;

        if (IsAjaxRequest())
        {
            return PartialView("_EditForm", dto);
        }

        return View(dto);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete([FromBody] DeleteEmployeeRequest request)
    {
        try
        {
            Console.WriteLine($"Delete action called with ID: {request?.Id}");

            if (request == null || request.Id <= 0)
            {
                return Json(new { success = false, message = "Invalid employee ID" });
            }

            await _employeeService.DeleteAsync(request.Id);
            return Json(new { success = true, message = "Employee deleted successfully" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in Delete action: {ex.Message}");
            return Json(new { success = false, message = ex.Message });
        }
    }

     public class DeleteEmployeeRequest
    {
        public int Id { get; set; }
    }
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ActionName("DeleteEmployee")]
    public async Task<IActionResult> DeleteEmployee(int employeeId)
    {
        try
        {
            Console.WriteLine($"DeleteEmployee action called with ID: {employeeId}");

            if (employeeId <= 0)
            {
                return Json(new { success = false, message = "Invalid employee ID" });
            }

            await _employeeService.DeleteAsync(employeeId);
            return Json(new { success = true, message = "Employee deleted successfully" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in DeleteEmployee action: {ex.Message}");
            return Json(new { success = false, message = ex.Message });
        }
    }
    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Search(string keyword)
    {
        var employees = await _employeeService.SearchAsync(keyword);

         if (User.IsInRole("Manager"))
        {
            var currentUser = await _userManager.GetUserAsync(User);
            employees = employees.Where(e => e.ManagerId == currentUser.Id).ToList();
        }

        if (IsAjaxRequest())
        {
            return PartialView("_EmployeesList", employees);
        }

        return Json(employees);
    }

    [HttpGet]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> ManagerDashboard()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return RedirectToAction("Login", "Auth");
        }

         var teamMembers = await _employeeService.GetEmployeesByManagerAsync(currentUser.Id);

        if (IsAjaxRequest())
        {
            return PartialView("_ManagerDashboard", teamMembers);
        }

        return View(teamMembers);
    }

    [HttpGet]
    [Authorize(Roles = "Employee,Manager,Admin")]
    public async Task<IActionResult> EmployeeDashboard()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        var employee = await _employeeService.GetByIdAsync(currentUser.Id);

        if (IsAjaxRequest())
        {
            return PartialView("_EmployeeDashboard", employee);
        }

        return View(employee);
    }

    [HttpGet]
    [Authorize(Roles = "Employee,Manager,Admin")]
    public async Task<IActionResult> UpdateTaskStatus(int taskId)
    {
         var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            if (IsAjaxRequest())
            {
                return Json(new { success = false, message = "User not found" });
            }

            return RedirectToAction("Login", "Auth");
        }

         var employee = await _employeeService.GetByIdAsync(currentUser.Id);

         var task = employee.Tasks.FirstOrDefault(t => t.Id == taskId);
        if (task == null)
        {
            if (IsAjaxRequest())
            {
                return Json(new { success = false, message = "Task not found or you don't have permission to update it." });
            }

            TempData["ErrorMessage"] = "Task not found or you don't have permission to update it.";
            return RedirectToAction("EmployeeDashboard");
        }

         ViewBag.TaskId = taskId;
        ViewBag.TaskTitle = task.Title;
        ViewBag.CurrentStatus = task.Status;

         ViewBag.StatusOptions = GetAllowedStatusTransitions(task.Status);

        if (IsAjaxRequest())
        {
            return PartialView("_UpdateTaskStatusForm", task);
        }

        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Employee,Manager,Admin")]
    public async Task<IActionResult> UpdateTaskStatus(int taskId, TaskState newStatus)
    {
         var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            if (IsAjaxRequest())
            {
                return Json(new { success = false, message = "User not found" });
            }

            return RedirectToAction("Login", "Auth");
        }

        try
        {
             var taskDto = await _taskService.GetTaskByIdAsync(taskId);
            if (taskDto == null || taskDto.EmployeeId != currentUser.Id)
            {
                if (IsAjaxRequest())
                {
                    return Json(new { success = false, message = "Task not found or you don't have permission to update it." });
                }

                TempData["ErrorMessage"] = "Task not found or you don't have permission to update it.";
                return RedirectToAction("EmployeeDashboard");
            }

             if (!IsValidStatusTransition(taskDto.Status, newStatus))
            {
                if (IsAjaxRequest())
                {
                    return Json(new { success = false, message = "Invalid status transition." });
                }

                TempData["ErrorMessage"] = "Invalid status transition.";
                return RedirectToAction("EmployeeDashboard");
            }

             taskDto.Status = newStatus;
            await _taskService.UpdateTaskAsync(taskDto);

            if (IsAjaxRequest())
            {
                return Json(new
                {
                    success = true,
                    message = $"Task status updated to {newStatus}.",
                    redirectUrl = Url.Action("EmployeeDashboard")
                });
            }

            TempData["SuccessMessage"] = $"Task status updated to {newStatus}.";
        }
        catch (Exception ex)
        {
            if (IsAjaxRequest())
            {
                return Json(new { success = false, message = $"Error updating task status: {ex.Message}" });
            }

            TempData["ErrorMessage"] = $"Error updating task status: {ex.Message}";
        }

        return RedirectToAction("EmployeeDashboard");
    }

    private List<SelectListItem> GetAllowedStatusTransitions(TaskState currentStatus)
    {
        var allowedStates = new List<TaskState>();

        switch (currentStatus)
        {
            case TaskState.Pending:
                allowedStates.Add(TaskState.InProgress);
                break;
            case TaskState.InProgress:
                allowedStates.Add(TaskState.Completed);
                allowedStates.Add(TaskState.Failed);
                break;
            case TaskState.Completed:
            case TaskState.Failed:
            case TaskState.Cancelled:
                 break;
        }

        return allowedStates
            .Select(s => new SelectListItem(s.ToString(), ((int)s).ToString()))
            .ToList();
    }

    private bool IsValidStatusTransition(TaskState currentStatus, TaskState newStatus)
    {
        switch (currentStatus)
        {
            case TaskState.Pending:
                return newStatus == TaskState.InProgress;
            case TaskState.InProgress:
                return newStatus == TaskState.Completed || newStatus == TaskState.Failed;
            case TaskState.Completed:
            case TaskState.Failed:
            case TaskState.Cancelled:
                return false;  
            default:
                return false;
        }
    }
}