using HostelMealManagement.Application.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HostelMealManagement.Web.Controllers;
[Authorize]
public class DashboardController(IMemberRepository memberRepository,IMealAttendanceRepository mealAttendanceRepository) : Controller
{
    [Route("/Dashboard")]
    public async Task<IActionResult> Index()
    {
        ViewBag.Members = memberRepository.GetMemberList().Count();
        ViewBag.TodayTotalMeal = await mealAttendanceRepository.GetTodayTotalMealAsync();
        return View();
    }
    [HttpGet("meal-trend")]
    public async Task<IActionResult> GetMealTrend(string type = "week")
    {
        var data = await mealAttendanceRepository.GetMealTrendAsync(type);
        return Json(data); // MVC friendly
    }

}
