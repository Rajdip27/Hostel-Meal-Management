using HostelMealManagement.Application.Repositories.Base;
using HostelMealManagement.Core.Entities;
using HostelMealManagement.Infrastructure.DatabaseContext;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HostelMealManagement.Application.Repositories;

public interface IUtilityBillRepository : IBaseService<UtilityBill>
{
    List<SelectListItem> GetUtilityBillMonthList();
}

public class UtilityBillRepository(ApplicationDbContext context)
    : BaseService<UtilityBill>(context), IUtilityBillRepository
{
    public List<SelectListItem> GetUtilityBillMonthList()
    {
        try
        {
            return _context.Set<UtilityBill>()
                .Where(x => !x.IsDelete)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = $"{x.Month} - {x.Date:yyyy}"
                })
                .ToList();
        }
        catch (Exception)
        {
            throw;
        }
    }
}
