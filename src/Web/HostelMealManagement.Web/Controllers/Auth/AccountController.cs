using HostelMealManagement.Application.Repositories.Auth;
using HostelMealManagement.Application.ViewModel;
using HostelMealManagement.Application.ViewModel.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static HostelMealManagement.Core.Entities.Auth.IdentityModel;

namespace HostelMealManagement.Web.Controllers.Auth;

public class AccountController(
    SignInManager<User> _signInManager,
    UserManager<User> _userManager,
    IAuthService _authService,
    IMemberRepository _memberRepository
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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            TempData["Error"] = "User not found. Please login again.";
            return RedirectToAction("Login");
        }

        var result = await _userManager.ChangePasswordAsync(
            user,
            model.CurrentPassword,
            model.NewPassword
        );

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            TempData["Error"] = "Current password is incorrect or new password is invalid.";
            return View(model);
        }

        await _signInManager.RefreshSignInAsync(user);

        TempData["Success"] = "Your password has been changed successfully.";

        return RedirectToAction("ChangePassword");
    }
    // ================= PROFILE =================

    // ================= PROFILE =================

    [HttpGet]
    [Authorize(Roles = "Member")]
    public async Task<IActionResult> Profile()
    {
        // 1️⃣ Get logged-in identity user
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return RedirectToAction("Login");

        // 2️⃣ Load member entity by UserId
        var memberEntity = (await _memberRepository.GetAllAsync())
            .FirstOrDefault(m => m.Id == user.MemberId);

        if (memberEntity == null)
            return NotFound("Member profile not found.");

        // 3️⃣ Map entity → ViewModel
        var model = new MemberVm
        {
            Id = memberEntity.Id,
            Name = memberEntity.Name,
            MemberCode = memberEntity.MemberCode,
            PhoneNumber = memberEntity.PhoneNumber,
            Email = memberEntity.Email,
            Picture = memberEntity.Picture,
            FatherName = memberEntity.FatherName,
            MotherName = memberEntity.MotherName,
            DateOfBirth = memberEntity.DateOfBirth,
            Gender = memberEntity.Gender,
            Religion = memberEntity.Religion,
            JoiningDate = memberEntity.JoiningDate,
            PermanentAddress = memberEntity.PermanentAddress,
            PresentAddress = memberEntity.PresentAddress,
            EmergencyName = memberEntity.EmergencyName,
            EmergencyContact = memberEntity.EmergencyContact,
            Relationship = memberEntity.Relationship,
            HouseBill = memberEntity.HouseBill,
            UtilityBill = memberEntity.UtilityBill,
            OtherBill = memberEntity.OtherBill,
            MealStatus = memberEntity.MealStatus
        };

        // 4️⃣ Return Profile view
        return View(model);
    }



}
