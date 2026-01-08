using AutoMapper;
using HostelMealManagement.Services;
using HostelMealManagement.Application.Logging;
using HostelMealManagement.Application.ViewModel;
using HostelMealManagement.Core.Entities;
using HostelMealManagement.Infrastructure.Helper.Acls;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static HostelMealManagement.Core.Entities.Auth.IdentityModel;

namespace HostelMealManagement.Web.Controllers;

[Authorize]
public class MemberController : Controller
{
    private readonly IMemberRepository _memberRepository;
    private readonly IMapper _mapper;
    private readonly IAppLogger<MemberController> _logger;
    private readonly ISignInHelper _signInHelper;
    private readonly UserManager<User> _userManager;
    private readonly IEmailService _emailService;

    public MemberController(
        IMemberRepository memberRepository,
        IMapper mapper,
        IAppLogger<MemberController> logger,
        ISignInHelper signInHelper,
        UserManager<User> userManager,
        IEmailService emailService)
    {
        _memberRepository = memberRepository;
        _mapper = mapper;
        _logger = logger;
        _signInHelper = signInHelper;
        _userManager = userManager;
        _emailService = emailService;
    }

    // ===========================
    // MEMBER LIST
    // ===========================
    [HttpGet("/Member")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        try
        {
            var roles = _signInHelper.Roles;
            var userId = _signInHelper.UserId;

            IEnumerable<Member> members;

            if (roles.Contains("Member") && userId.HasValue)
            {
                var user = await _userManager.FindByIdAsync(userId.Value.ToString());

                if (user?.MemberId != null)
                {
                    var member = await _memberRepository.FindAsync(
                        m => m.Id == user.MemberId.Value);

                    members = member != null
                        ? new List<Member> { member }
                        : new List<Member>();
                }
                else
                {
                    members = new List<Member>();
                }
            }
            else
            {
                members = await _memberRepository.GetAllAsync();
            }

            var memberVmList = _mapper.Map<List<MemberVm>>(members);
            return View(memberVmList);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error fetching members", ex);
            return StatusCode(500, "An error occurred while fetching members.");
        }
    }

    // ===========================
    // CREATE / EDIT (GET)
    // ===========================
    [HttpGet]
    [Route("Member/CreateOrEdit/{Id?}")]
    public async Task<IActionResult> CreateOrEdit(long Id = 0)
    {
        var vm = new MemberVm();

        if (Id > 0)
        {
            var member = await _memberRepository.FindAsync(Id);

            if (member == null)
            {
                TempData["AlertMessage"] = $"Member with Id {Id} not found.";
                TempData["AlertType"] = "Error";
                return RedirectToAction(nameof(Index));
            }

            vm = _mapper.Map<MemberVm>(member);
        }

        return View(vm);
    }

    // ===========================
    // CREATE / EDIT (POST)
    // ===========================
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("Member/CreateOrEdit/{Id?}")]
    public async Task<IActionResult> CreateOrEdit(MemberVm vm)
    {
        // ================= FIX (CONTROLLER ONLY) =================
        ModelState.Remove("ImageFile");
        ModelState.Remove("Password");
        // =========================================================

        if (!ModelState.IsValid)
        {
            TempData["AlertMessage"] = "Please fix the validation errors.";
            TempData["AlertType"] = "Warning";
            return View(vm);
        }

        try
        {
            var isNewMember = vm.Id == 0;

            // 🔹 SAVE MEMBER (NO LOGIC CHANGE)
            var result = await _memberRepository
                .CreateOrUpdateMemberWithUserAsync(
                    vm,
                    HttpContext.RequestAborted);

            _logger.LogInfo(
                $"CreateOrUpdateMemberWithUserAsync result: {result}, Email: {vm.Email}");

            if (!result)
            {
                TempData["AlertMessage"] = isNewMember
                    ? "Failed to create member."
                    : $"Member with Id {vm.Id} not found.";

                TempData["AlertType"] = "Error";
                return View(vm);
            }

            // ================= EMAIL AFTER REGISTRATION =================
            if (result)
            {
                var user = await _userManager.FindByEmailAsync(vm.Email);

                if (user != null)
                {
                    var subject = "Your Hostel Account Has Been Created";

                    var body = $@"
                        <h3>Welcome, {vm.Name}</h3>
                        <p>Your registration has been completed by the Admin.</p>
                        <p><b>Email:</b> {vm.Email}</p>
                        <p><b>Password:</b> {vm.Password}</p>
                        <p>Please change your password after first login.</p>
                        <br/>
                        <p>Hostel Meal Management System</p>
                    ";

                    await _emailService.SendAsync(
                        vm.Email,
                        subject,
                        body);

                    _logger.LogInfo(
                        $"Registration email sent successfully to {vm.Email}");
                }
                else
                {
                    _logger.LogWarning(
                        $"User not found after creation. Email not sent. Email: {vm.Email}");
                }
            }
            // ============================================================

            TempData["AlertMessage"] = isNewMember
                ? "Member created successfully!"
                : "Member updated successfully!";

            TempData["AlertType"] = "Success";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while creating/updating member", ex);

            TempData["AlertMessage"] = "An error occurred while saving the member.";
            TempData["AlertType"] = "Error";

            return View(vm);
        }
    }
}
