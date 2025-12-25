using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HostelMealManagement.Web.Controllers;
[Authorize]
public class DashboardController(IMemberRepository memberRepository) : Controller
{
    [Route("/Dashboard")]
    public IActionResult Index()
    {
        ViewBag.Members = memberRepository.GetMemberList().Count();
        return View();
    }
}
