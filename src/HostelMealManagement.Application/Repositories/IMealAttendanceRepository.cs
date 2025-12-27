using HostelMealManagement.Application.Repositories.Base;
using HostelMealManagement.Application.ViewModel;
using HostelMealManagement.Core.Entities;
using HostelMealManagement.Infrastructure.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace HostelMealManagement.Application.Repositories;

public interface IMealAttendanceRepository : IBaseService<MealAttendance>
{
    Task<bool> UpsertAsync(MealAttendanceVm vm);
    Task<MealAttendanceVm> GetByIdAsync(long id);
    Task<List<MealAttendanceVm>> GetAllAsync();
    Task<bool> DeleteAsync(long id);
    Task<int> GetTodayTotalMealAsync();
    Task<List<MealTrendDto>> GetMealTrendAsync(string type);
}

public class MealAttendanceRepository(ApplicationDbContext context)
    : BaseService<MealAttendance>(context), IMealAttendanceRepository
{
    // ----------------------------------------------------------------------
    // UPSERT
    // ----------------------------------------------------------------------
    public async Task<bool> UpsertAsync(MealAttendanceVm vm)
    {
        using var trx = await context.Database.BeginTransactionAsync();

        try
        {
            MealAttendance entity;

            // ========== CREATE ==========
            if (vm.Id == 0)
            {
                entity = new MealAttendance
                {
                    MemberId = vm.MemberId,
                    MealDate = vm.MealDate,

                    IsBreakfast = vm.IsBreakfast,
                    IsLunch = vm.IsLunch,
                    IsDinner = vm.IsDinner,

                    IsGuest = vm.IsGuest,
                    GuestIsBreakfast = vm.GuestIsBreakfast,
                    GuestBreakfastQty = vm.GuestBreakfastQty,
                    GuestIsLunch = vm.GuestIsLunch,
                    GuestLunchQty = vm.GuestLunchQty,
                    GuestIsDinner = vm.GuestIsDinner,
                    GuestDinnerQty = vm.GuestDinnerQty
                };

                context.Set<MealAttendance>().Add(entity);
                await context.SaveChangesAsync();
                await trx.CommitAsync();
                return true;
            }

            // ========== UPDATE ==========
            entity = await context.Set<MealAttendance>()
                .FirstOrDefaultAsync(a => a.Id == vm.Id);

            if (entity == null)
                return false;

            entity.MemberId = vm.MemberId;
            entity.MealDate = vm.MealDate;

            entity.IsBreakfast = vm.IsBreakfast;
            entity.IsLunch = vm.IsLunch;
            entity.IsDinner = vm.IsDinner;

            entity.IsGuest = vm.IsGuest;
            entity.GuestIsBreakfast = vm.GuestIsBreakfast;
            entity.GuestBreakfastQty = vm.GuestBreakfastQty;
            entity.GuestIsLunch = vm.GuestIsLunch;
            entity.GuestLunchQty = vm.GuestLunchQty;
            entity.GuestIsDinner = vm.GuestIsDinner;
            entity.GuestDinnerQty = vm.GuestDinnerQty;

            await context.SaveChangesAsync();
            await trx.CommitAsync();
            return true;
        }
        catch
        {
            await trx.RollbackAsync();
            return false;
        }
    }

    // ----------------------------------------------------------------------
    // GET BY ID
    // ----------------------------------------------------------------------
    public async Task<MealAttendanceVm?> GetByIdAsync(long id)
    {
        var entity = await context.Set<MealAttendance>()
            .Include(a => a.Member)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (entity == null)
            return null;

        return new MealAttendanceVm
        {
            Id = entity.Id,
            MemberId = entity.MemberId,
            MemberName = entity.Member?.Name,
            MealDate = entity.MealDate,

            IsBreakfast = entity.IsBreakfast,
            IsLunch = entity.IsLunch,
            IsDinner = entity.IsDinner,

            IsGuest = entity.IsGuest,
            GuestIsBreakfast = entity.GuestIsBreakfast,
            GuestBreakfastQty = entity.GuestBreakfastQty,
            GuestIsLunch = entity.GuestIsLunch,
            GuestLunchQty = entity.GuestLunchQty,
            GuestIsDinner = entity.GuestIsDinner,
            GuestDinnerQty = entity.GuestDinnerQty
        };
    }

    // ----------------------------------------------------------------------
    // DELETE
    // ----------------------------------------------------------------------
    public async Task<bool> DeleteAsync(long id)
    {
        using var trx = await context.Database.BeginTransactionAsync();

        try
        {
            var entity = await context.Set<MealAttendance>()
                .FirstOrDefaultAsync(a => a.Id == id);

            if (entity == null)
                return false;

            context.Set<MealAttendance>().Remove(entity);
            await context.SaveChangesAsync();

            await trx.CommitAsync();
            return true;
        }
        catch
        {
            await trx.RollbackAsync();
            return false;
        }
    }

    // ----------------------------------------------------------------------
    // GET ALL
    // ----------------------------------------------------------------------
    public async Task<List<MealAttendanceVm>> GetAllAsync()
    {
        return await context.Set<MealAttendance>()
            .Include(a => a.Member)
            .OrderByDescending(a => a.MealDate)
            .Select(a => new MealAttendanceVm
            {
                Id = a.Id,
                MemberId = a.MemberId,
                MemberName = a.Member.Name,
                MealDate = a.MealDate,

                IsBreakfast = a.IsBreakfast,
                IsLunch = a.IsLunch,
                IsDinner = a.IsDinner,

                IsGuest = a.IsGuest,
                GuestIsBreakfast = a.GuestIsBreakfast,
                GuestBreakfastQty = a.GuestBreakfastQty,
                GuestIsLunch = a.GuestIsLunch,
                GuestLunchQty = a.GuestLunchQty,
                GuestIsDinner = a.GuestIsDinner,
                GuestDinnerQty = a.GuestDinnerQty
            })
            .ToListAsync();
    }

    public async Task<int> GetTodayTotalMealAsync()
    {
        var today = DateTimeOffset.Now.Date;

        var totalMeal = await _context.Set<MealAttendance>()
            .Where(x => x.MealDate.Date == today)
            .SumAsync(x =>
                (x.IsBreakfast ? 1 : 0) +
                (x.IsLunch ? 1 : 0) +
                (x.IsDinner ? 1 : 0) +
                (x.GuestIsBreakfast ? x.GuestBreakfastQty : 0) +
                (x.GuestIsLunch ? x.GuestLunchQty : 0) +
                (x.GuestIsDinner ? x.GuestDinnerQty : 0)
            );

        return totalMeal;
    }

    public async Task<List<MealTrendDto>> GetMealTrendAsync(string type)
    {
        var query = _context.Set<MealAttendance>().AsQueryable();

        if (type == "week")
        {
            var start = DateTime.Now.Date.AddDays(-6);
            var end = DateTime.Now.Date;

            // Get raw data from DB
            var data = await query
                .Where(x => x.MealDate.Date >= start && x.MealDate.Date <= end)
                .GroupBy(x => x.MealDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    TotalMeal = g.Sum(x =>
                        (x.IsBreakfast ? 1 : 0) +
                        (x.IsLunch ? 1 : 0) +
                        (x.IsDinner ? 1 : 0) +
                        (x.GuestIsBreakfast ? x.GuestBreakfastQty : 0) +
                        (x.GuestIsLunch ? x.GuestLunchQty : 0) +
                        (x.GuestIsDinner ? x.GuestDinnerQty : 0))
                })
                .ToListAsync();

            // Fill missing days with 0
            var result = new List<MealTrendDto>();
            for (var day = start; day <= end; day = day.AddDays(1))
            {
                var dayData = data.FirstOrDefault(x => x.Date == day);
                result.Add(new MealTrendDto
                {
                    Label = day.ToString("ddd", CultureInfo.InvariantCulture), // Mon, Tue...
                    TotalMeal = dayData?.TotalMeal ?? 0
                });
            }

            return result;
        }

        // Month
        if (type == "month")
        {
            var data = await query
                .GroupBy(x => x.MealDate.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    TotalMeal = g.Sum(x =>
                        (x.IsBreakfast ? 1 : 0) +
                        (x.IsLunch ? 1 : 0) +
                        (x.IsDinner ? 1 : 0) +
                        (x.GuestIsBreakfast ? x.GuestBreakfastQty : 0) +
                        (x.GuestIsLunch ? x.GuestLunchQty : 0) +
                        (x.GuestIsDinner ? x.GuestDinnerQty : 0))
                })
                .OrderBy(x => x.Month)
                .ToListAsync();

            return data.Select(x => new MealTrendDto
            {
                Label = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x.Month),
                TotalMeal = x.TotalMeal
            }).ToList();
        }

        // Year
        var yearData = await query
            .GroupBy(x => x.MealDate.Year)
            .Select(g => new
            {
                Year = g.Key,
                TotalMeal = g.Sum(x =>
                    (x.IsBreakfast ? 1 : 0) +
                    (x.IsLunch ? 1 : 0) +
                    (x.IsDinner ? 1 : 0) +
                    (x.GuestIsBreakfast ? x.GuestBreakfastQty : 0) +
                    (x.GuestIsLunch ? x.GuestLunchQty : 0) +
                    (x.GuestIsDinner ? x.GuestDinnerQty : 0))
            })
            .OrderBy(x => x.Year)
            .ToListAsync();

        return yearData.Select(x => new MealTrendDto
        {
            Label = x.Year.ToString(),
            TotalMeal = x.TotalMeal
        }).ToList();
    }

}