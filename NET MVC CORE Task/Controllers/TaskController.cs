using ARIBApp.Core.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace NET_MVC_CORE_Task.Controllers
{
    [Authorize]
    public class TaskController : Controller
    {
        private readonly ITaskService _taskService;
        private readonly IEmployeeService _employeeService;
        private readonly UserManager<Employee> _userManager;

        public TaskController(
            ITaskService taskService,
            IEmployeeService employeeService,
            UserManager<Employee> userManager)
        {
            _taskService = taskService;
            _employeeService = employeeService;
            _userManager = userManager;
        }
        private bool IsAjaxRequest()
        {
            return Request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }



        [HttpGet]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> EmployeeTasks(int employeeId)
        {
            var employee = await _employeeService.GetByIdAsync(employeeId);
            if (employee == null)
                return NotFound();

            if (User.IsInRole("Manager"))
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null || employee.ManagerId != currentUser.Id)
                    return Forbid();
            }

            ViewBag.Employee = employee;

            if (IsAjaxRequest())
            {
                return PartialView("_EmployeeTasks", employee.Tasks);
            }

            return View(employee.Tasks);
        }

        [HttpGet]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> Create(int employeeId)
        {
             var employee = await _employeeService.GetByIdAsync(employeeId);
            if (employee == null)
                return NotFound();

             if (User.IsInRole("Manager"))
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null || employee.ManagerId != currentUser.Id)
                    return Forbid();
            }

            ViewBag.Employee = employee;
            ViewBag.StatusOptions = Enum.GetValues<TaskState>()
                .Select(s => new SelectListItem(s.ToString(), ((int)s).ToString()))
                .ToList();

            var newTask = new TaskDTO
            {
                EmployeeId = employeeId,
                Status = TaskState.Pending,
                CreatedAt = DateTime.UtcNow
            };

            if (IsAjaxRequest())
            {
                return PartialView("_CreateTaskForm", newTask);
            }

            return View(newTask);
        }

        [HttpPost]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> Create(TaskDTO dto)
        {
            if (!ModelState.IsValid)
            {
                var employee = await _employeeService.GetByIdAsync(dto.EmployeeId);
                ViewBag.Employee = employee;
                ViewBag.StatusOptions = Enum.GetValues<TaskState>()
                    .Select(s => new SelectListItem(s.ToString(), ((int)s).ToString(), s == dto.Status))
                    .ToList();

                if (IsAjaxRequest())
                {
                    return PartialView("_CreateTaskForm", dto);
                }

                return View(dto);
            }

             var employeeCheck = await _employeeService.GetByIdAsync(dto.EmployeeId);
            if (employeeCheck == null)
                return NotFound();

             if (User.IsInRole("Manager"))
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null || employeeCheck.ManagerId != currentUser.Id)
                    return Forbid();
            }

            await _taskService.AssignTaskAsync(dto);
            TempData["SuccessMessage"] = "Task created successfully!";

            if (IsAjaxRequest())
            {
                return Json(new
                {
                    success = true,
                    message = "Task created successfully!",
                    redirectUrl = Url.Action("EmployeeTasks", new { employeeId = dto.EmployeeId })
                });
            }

            return RedirectToAction("EmployeeTasks", new { employeeId = dto.EmployeeId });
        }

        [HttpGet]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> Edit(int id, int employeeId)
        {
            var tasks = await _taskService.GetTasksForEmployeeAsync(employeeId);
            if (tasks == null || !tasks.Any(t => t.Id == id))
                return NotFound();

            var task = tasks.FirstOrDefault(t => t.Id == id);
            if (task == null)
                return NotFound();

            var employee = await _employeeService.GetByIdAsync(employeeId);
            if (employee == null)
                return NotFound();

             if (User.IsInRole("Manager"))
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null || employee.ManagerId != currentUser.Id)
                    return Forbid();

                 var managedEmployees = await _employeeService.GetEmployeesByManagerAsync(currentUser.Id);
                ViewBag.ManagedEmployees = managedEmployees;
            }
            else if (User.IsInRole("Admin"))
            {
                 var allEmployees = await _employeeService.GetAllAsync();
                ViewBag.ManagedEmployees = allEmployees;
            }

            ViewBag.Employee = employee;
            ViewBag.StatusOptions = Enum.GetValues<TaskState>()
                .Select(s => new SelectListItem(s.ToString(), ((int)s).ToString(), s == task.Status))
                .ToList();

            if (IsAjaxRequest())
            {
                return PartialView("_EditTaskForm", task);
            }

            return View(task);
        }

        [HttpPost]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> Edit(TaskDTO dto, int employeeId)
        {
            if (!ModelState.IsValid)
            {
                var employee = await _employeeService.GetByIdAsync(employeeId);
                ViewBag.Employee = employee;

                 if (User.IsInRole("Manager"))
                {
                    var currentUser = await _userManager.GetUserAsync(User);
                    if (currentUser != null)
                    {
                        var managedEmployees = await _employeeService.GetEmployeesByManagerAsync(currentUser.Id);
                        ViewBag.ManagedEmployees = managedEmployees;
                    }
                }
                else if (User.IsInRole("Admin"))
                {
                    var allEmployees = await _employeeService.GetAllAsync();
                    ViewBag.ManagedEmployees = allEmployees;
                }

                ViewBag.StatusOptions = Enum.GetValues<TaskState>()
                    .Select(s => new SelectListItem(s.ToString(), ((int)s).ToString(), s == dto.Status))
                    .ToList();

                if (IsAjaxRequest())
                {
                    return PartialView("_EditTaskForm", dto);
                }

                return View(dto);
            }

             var currentEmployee = await _employeeService.GetByIdAsync(employeeId);
            if (currentEmployee == null)
                return NotFound();

             var targetEmployee = await _employeeService.GetByIdAsync(dto.EmployeeId);
            if (targetEmployee == null)
                return NotFound();

             if (User.IsInRole("Manager"))
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null ||
                    currentEmployee.ManagerId != currentUser.Id ||
                    targetEmployee.ManagerId != currentUser.Id)
                {
                    return Forbid();
                }
            }

            try
            {
                await _taskService.UpdateTaskAsync(dto);
                TempData["SuccessMessage"] = "Task updated successfully!";

                if (IsAjaxRequest())
                {
                    return Json(new
                    {
                        success = true,
                        message = "Task updated successfully!",
                        redirectUrl = Url.Action("EmployeeTasks", new { employeeId = dto.EmployeeId })
                    });
                }

                 return RedirectToAction("EmployeeTasks", new { employeeId = dto.EmployeeId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating task: {ex.Message}";

                if (IsAjaxRequest())
                {
                    return Json(new
                    {
                        success = false,
                        message = $"Error updating task: {ex.Message}"
                    });
                }

                return RedirectToAction("EmployeeTasks", new { employeeId });
            }
        }


    }
}