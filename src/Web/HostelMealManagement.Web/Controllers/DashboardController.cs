using HostelMealManagement.Application.Repositories;
using HostelMealManagement.Application.ViewModel;
using HostelMealManagement.Infrastructure.Helper.Acls;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HostelMealManagement.Web.Controllers;
[Authorize]
public class DashboardController(IMemberRepository memberRepository,IMealAttendanceRepository mealAttendanceRepository, ISignInHelper signInHelper) : Controller
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
        List<MealTrendDto> data = new List<MealTrendDto>();
        if (signInHelper.Roles.Contains("Member"))
            data = await mealAttendanceRepository.GetMealTrendAsync(type, signInHelper.UserId ?? 0);
        else 
            data = await mealAttendanceRepository.GetMealTrendAsync(type, 0);
        
        return Json(data); // MVC friendly
    }

}
