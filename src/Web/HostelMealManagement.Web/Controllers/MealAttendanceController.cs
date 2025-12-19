using Microsoft.AspNetCore.Mvc;

namespace HostelMealManagement.Web.Controllers;

public class MealAttendanceController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
