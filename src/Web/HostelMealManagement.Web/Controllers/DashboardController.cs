using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HostelMealManagement.Web.Controllers;
[Authorize]
public class DashboardController : Controller
{
    [Route("/Dashboard")]
    public IActionResult Index()
    {
        return View();
    }
}
