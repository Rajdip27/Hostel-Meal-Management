using AutoMapper;
using HostelMealManagement.Application.Logging;
using HostelMealManagement.Application.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HostelMealManagement.Web.Controllers;

public class MemberController(IMemberRepository memberRepository, IMapper mapper, IAppLogger<MemberController> logger) : Controller
{
    [HttpGet("/Member")]
    public async Task<IActionResult> Index()
    {
        try
        {
            #if DEBUG
            logger.LogInfo($"Start Watch");
            var stopwatch = Stopwatch.StartNew();
            #endif
            // Always fetch fresh data (real-time)
            var pagination = await memberRepository.GetAllAsync();
            var memberVmList = mapper.Map<List<MemberVm>>(pagination);
            #if DEBUG
            logger.LogInfo($"GetUserData took {stopwatch.ElapsedMilliseconds}ms");
            #endif
            logger.LogInfo($"Fetched {pagination.Count()} categories");
            return View(memberVmList);
        }
        catch (Exception ex)
        {
            logger.LogError("Error while fetching categories", ex);
            return StatusCode(500, "An error occurred while fetching categories.");
        }
    }
    [HttpGet]
    [Route("Member/CreateOrEdit/{Id?}")]
    public IActionResult CreateOrEdit(long Id)
    {
        MemberVm vm = new MemberVm();
        if (Id> 0)
        {
            // Edit existing member
            var member = memberRepository.Find(Id);
            vm = mapper.Map<MemberVm>(member);
        }
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("Member/CreateOrEdit/{Id?}")]
    public async Task<IActionResult> CreateOrEdit(MemberVm categoryVm)
    {
        if (!ModelState.IsValid)
        {
            TempData["AlertMessage"] = "Please fix validation errors.";
            TempData["AlertType"] = "Warning";
            return View(categoryVm);
        }

        try
        {
            var result = await memberRepository.CreateOrUpdateMemberWithUserAsync(categoryVm, HttpContext.RequestAborted);

            if (!result)
            {
                TempData["AlertMessage"] = $"Member with Id {categoryVm.Id} not found.";
                TempData["AlertType"] = "Error";
                return NotFound();
            }

            TempData["AlertMessage"] = categoryVm.Id > 0
                ? "Member updated successfully!"
                : "Member created successfully!";
            TempData["AlertType"] = "Success";

            // Redirect ensures Index fetches fresh data
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            logger.LogError("Error while creating/updating category", ex);
            TempData["AlertMessage"] = "An error occurred while saving the category.";
            TempData["AlertType"] = "Error";
            return StatusCode(500);
        }
    }

}
