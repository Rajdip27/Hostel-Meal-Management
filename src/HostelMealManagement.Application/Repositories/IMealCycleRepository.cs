using HostelMealManagement.Application.Repositories.Base;
using HostelMealManagement.Core.Entities;
using HostelMealManagement.Infrastructure.DatabaseContext;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HostelMealManagement.Application.Repositories;

public interface IMealCycleRepository : IBaseService<MealCycle>
{
    List<SelectListItem> GetMealCycleList();
}

public class MealCycleRepository(ApplicationDbContext context) : BaseService<MealCycle>(context), IMealCycleRepository
{
    public List<SelectListItem> GetMealCycleList()
    {
        try
        {
            return _context.Set<MealCycle>()
                 .Where(x => !x.IsDelete)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                })
                .ToList();
        }
        catch (Exception ex)
        {

            throw;
        }

    }
}
