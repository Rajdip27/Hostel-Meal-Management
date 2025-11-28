using AutoMapper;
using HostelMealManagement.Application.Logging;
using HostelMealManagement.Application.ViewModel;
using HostelMealManagement.Core.Entities;
using HostelMealManagement.Infrastructure.Helper.Acls;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using static HostelMealManagement.Core.Entities.Auth.IdentityModel;

namespace HostelMealManagement.Web.Controllers;

public class MemberController : Controller
{
    private readonly IMemberRepository _memberRepository;
    private readonly IMapper _mapper;
    private readonly IAppLogger<MemberController> _logger;
    private ISignInHelper SignInHelper;
    private readonly UserManager<User> _userManager;

    public MemberController(IMemberRepository memberRepository, IMapper mapper, IAppLogger<MemberController> logger, ISignInHelper signInHelper, UserManager<User> userManager)
    {
        _memberRepository = memberRepository;
        _mapper = mapper;
        _logger = logger;
        SignInHelper = signInHelper;
        _userManager = userManager;
    }
    [HttpGet("/Member")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        try
        {
            var roles = SignInHelper.Roles;
            var userId = SignInHelper.UserId;

            IEnumerable<Member> members;

            if (roles.Contains("Member") && userId.HasValue)
            {
                // Get the User record
                var user = await _userManager.FindByIdAsync(userId.Value.ToString());

                if (user?.MemberId != null)
                {
                    // Get the Member associated with this user
                    var member = await _memberRepository.FindAsync(m => m.Id == user.MemberId.Value);
                    members = member != null ? new List<Member> { member } : new List<Member>();
                }
                else
                {
                    members = new List<Member>();
                }
            }
            else
            {
                // Admin/Manager → fetch all members
                members = await _memberRepository.GetAllAsync();
            }

            var memberVmList = _mapper.Map<List<MemberVm>>(members);
            return View(memberVmList);
        }
        catch (Exception ex)
        {
            _logger.LogError( "Error fetching members", ex);
            return StatusCode(500, "An error occurred while fetching members.");
        }
    }



    [HttpGet]
    [Route("Member/CreateOrEdit/{Id?}")]
    public async Task<IActionResult> CreateOrEdit(long Id = 0)
    {
        var vm = new MemberVm();

        if (Id > 0)
        {
            // Edit existing member
            var member = await _memberRepository.FindAsync(Id);
            if (member != null)
                vm = _mapper.Map<MemberVm>(member);
            else
            {
                TempData["AlertMessage"] = $"Member with Id {Id} not found.";
                TempData["AlertType"] = "Error";
                return RedirectToAction(nameof(Index));
            }
        }

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("Member/CreateOrEdit/{Id?}")]
    public async Task<IActionResult> CreateOrEdit(MemberVm vm)
    {
        if (!ModelState.IsValid)
        {
            TempData["AlertMessage"] = "Please fix the validation errors.";
            TempData["AlertType"] = "Warning";
            return View(vm);
        }

        try
        {
            var result = await _memberRepository.CreateOrUpdateMemberWithUserAsync(vm, HttpContext.RequestAborted);

            if (!result)
            {
                TempData["AlertMessage"] = vm.Id > 0
                    ? $"Member with Id {vm.Id} not found."
                    : "Failed to create member.";
                TempData["AlertType"] = "Error";
                return RedirectToAction(nameof(Index));
            }

            TempData["AlertMessage"] = vm.Id > 0
                ? "Member updated successfully!"
                : "Member created successfully!";
            TempData["AlertType"] = "Success";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while creating/updating member", ex);
            TempData["AlertMessage"] = "An error occurred while saving the member.";
            TempData["AlertType"] = "Error";
            return StatusCode(500);
        }
    }
}
