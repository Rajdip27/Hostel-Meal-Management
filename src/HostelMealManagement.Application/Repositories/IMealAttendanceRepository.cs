using HostelMealManagement.Application.Repositories.Base;
using HostelMealManagement.Application.ViewModel;
using HostelMealManagement.Core.Entities;
using HostelMealManagement.Infrastructure.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace HostelMealManagement.Application.Repositories;

public interface IMealAttendanceRepository : IBaseService<MealAttendance>
{
    Task<bool> UpsertAsync(MealAttendanceVm vm);
    Task<MealAttendanceVm?> GetByIdAsync(long id);
    Task<List<MealAttendanceVm>> GetAllAsync();
    Task<bool> DeleteAsync(long id);
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
}