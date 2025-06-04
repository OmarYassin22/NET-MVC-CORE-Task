using ARIBApp.Core.Contracts;
using ARIBApp.Data.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace NET_MVC_CORE_Task.Controllers
{
    public class AuthController : Controller
    {
        private readonly SignInManager<Employee> _signInManager;
        private readonly UserManager<Employee> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public AuthController(
            SignInManager<Employee> signInManager,
            UserManager<Employee> userManager,
            RoleManager<IdentityRole<int>> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO loginDto)
        {
             var user = await _userManager.FindByNameAsync(loginDto.Username);
            if (user == null)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "User not found. Please check your username." });
                }

                ModelState.AddModelError("", "User not found. Please check your username.");
                return View();
            }

             var result = await _signInManager.PasswordSignInAsync(user.UserName, loginDto.Password, false, false);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                if (result.Succeeded)
                {
                     HttpContext.Session.SetInt32("LoggedInEmployeeId", user.Id);
                    return Json(new { success = true });
                }

                string errorMessage = "Invalid username/password.";
                if (result.IsLockedOut)
                    errorMessage = "Account is locked out.";
                else if (result.IsNotAllowed)
                    errorMessage = "Account is not allowed to sign in.";

                return Json(new { success = false, message = errorMessage });
            }

            if (result.Succeeded)
            {
                 HttpContext.Session.SetInt32("LoggedInEmployeeId", user.Id);
                return RedirectToAction("Index", "Employee");
            }

            if (result.IsLockedOut)
                ModelState.AddModelError("", "Account is locked out.");
            else if (result.IsNotAllowed)
                ModelState.AddModelError("", "Account is not allowed to sign in.");
            else
                ModelState.AddModelError("", "Invalid username/password.");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

         [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Register() => View();

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register(string email, string password, string role)
        {
            var user = new Employee { UserName = email, Email = email };
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                    await _roleManager.CreateAsync(new IdentityRole<int>(role));
                await _userManager.AddToRoleAsync(user, role);
                return RedirectToAction("Index", "Employee");
            }
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);
            return View();
        }
    }
}