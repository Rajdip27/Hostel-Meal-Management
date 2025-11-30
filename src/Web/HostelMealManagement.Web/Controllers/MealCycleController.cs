using Microsoft.AspNetCore.Mvc;

namespace HostelMealManagement.Web.Controllers;

public class MealCycleController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
