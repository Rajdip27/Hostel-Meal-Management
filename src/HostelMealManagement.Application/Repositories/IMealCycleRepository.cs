using HostelMealManagement.Application.Repositories.Base;
using HostelMealManagement.Core.Entities;
using HostelMealManagement.Infrastructure.DatabaseContext;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HostelMealManagement.Application.Repositories;

public interface IMealCycleRepository : IBaseService<MealCycle>
{
    List<SelectListItem> GetMealCycleList();
    Task<(DateTimeOffset StartDate, DateTimeOffset EndDate)> GetMealCycleDatesAsync(long mealCycleId);
    Task<decimal> GetTotalPaymentAmountAsync(
   long memberId,
   DateTimeOffset startDate,
   DateTimeOffset endDate);
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
    public async Task<(DateTimeOffset StartDate, DateTimeOffset EndDate)> GetMealCycleDatesAsync(long mealCycleId)
    {
        return await _context.Set<MealCycle>()
            .Where(x => x.Id == mealCycleId)
            .Select(x => new ValueTuple<DateTimeOffset, DateTimeOffset>(
                x.StartDate,
                x.EndDate
            ))
            .FirstOrDefaultAsync();
    }

    public async Task<decimal> GetTotalPaymentAmountAsync(
    long memberId,
    DateTimeOffset startDate,
    DateTimeOffset endDate)
    {
        return await _context.Set<NormalPayment>()
        .Where(x =>
            x.MemberId == memberId &&
            x.PaymentDate >= startDate &&
            x.PaymentDate <= endDate)
        .Select(x => (decimal?)x.PaymentAmount)   // nullable
        .SumAsync() ?? 0m;
    }


}
