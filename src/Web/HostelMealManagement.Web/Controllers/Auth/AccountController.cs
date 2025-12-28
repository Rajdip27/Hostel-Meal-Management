using HostelMealManagement.Application.Repositories.Auth;
using HostelMealManagement.Application.ViewModel.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static HostelMealManagement.Core.Entities.Auth.IdentityModel;

namespace HostelMealManagement.Web.Controllers.Auth;

public class AccountController(
    SignInManager<User> _signInManager,
    UserManager<User> _userManager,
    IAuthService _authService
) : Controller
{
    // ================= REGISTER =================

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = "/Dashboard")
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _authService.Register(model);

        if (!result.Success)
        {
            result.Errors.ForEach(e => ModelState.AddModelError("", e));
            return View(model);
        }

        var user = await _signInManager.UserManager
            .FindByIdAsync(result.UserId.ToString());

        if (user != null)
        {
            await _signInManager.SignInAsync(user, false);
        }

        return LocalRedirect(returnUrl);
    }

    // ================= LOGIN =================

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(
            model.Email,
            model.Password,
            false,
            lockoutOnFailure: false
        );

        if (result.Succeeded)
        {
            return LocalRedirect("/Dashboard");
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View(model);
    }

    // ================= LOGOUT =================

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login", "Account");
    }

    // ================= CHANGE PASSWORD =================

    [HttpGet]
    [Authorize] // Admin OR Member
    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return RedirectToAction("Login");

        var result = await _userManager.ChangePasswordAsync(
            user,
            model.CurrentPassword,
            model.NewPassword
        );

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }

        // keep user logged in
        await _signInManager.RefreshSignInAsync(user);

        TempData["Success"] = "Password changed successfully!";
        return RedirectToAction("Dashboard"); // or Profile
    }
}
