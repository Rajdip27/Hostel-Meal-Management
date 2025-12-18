using HostelMealManagement.Application.Repositories.Base;
using HostelMealManagement.Application.ViewModel;
using HostelMealManagement.Core.Entities;
using HostelMealManagement.Infrastructure.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace HostelMealManagement.Application.Repositories;

public interface IElectricBillRepository : IBaseService<ElectricBill>
{
    Task<bool> UpsertAsync(ElectricBillVm vm);
    Task<ElectricBillVm?> GetByIdAsync(long id);
    Task<List<ElectricBillVm>> GetAllAsync();
    Task<bool> DeleteAsync(long id);
}

public class ElectricBillRepository(ApplicationDbContext context)
    : BaseService<ElectricBill>(context), IElectricBillRepository
{
    // =====================================================
    // UPSERT
    // =====================================================
    public async Task<bool> UpsertAsync(ElectricBillVm vm)
    {
        using var trx = await context.Database.BeginTransactionAsync();
        try
        {
            ElectricBill entity;

            if (vm.Id == 0)
            {
                entity = new ElectricBill
                {
                    MealCycleId = vm.MealCycleId,
                    PreviousUnit = vm.PreviousUnit,
                    CurrentUnit = vm.CurrentUnit,
                    PerUnitRate = vm.PerUnitRate
                };

                context.Set<ElectricBill>().Add(entity);
            }
            else
            {
                entity = await context.Set<ElectricBill>()
                    .FirstOrDefaultAsync(x => x.Id == vm.Id);

                if (entity == null)
                    return false;

                entity.MealCycleId = vm.MealCycleId;
                entity.PreviousUnit = vm.PreviousUnit;
                entity.CurrentUnit = vm.CurrentUnit;
                entity.PerUnitRate = vm.PerUnitRate;
            }

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

    // =====================================================
    // GET BY ID
    // =====================================================
    public async Task<ElectricBillVm?> GetByIdAsync(long id)
    {
        var entity = await context.Set<ElectricBill>()
            .Include(x => x.MealCycle)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (entity == null)
            return null;

        return new ElectricBillVm
        {
            Id = entity.Id,
            MealCycleId = entity.MealCycleId,
           

            PreviousUnit = entity.PreviousUnit,
            CurrentUnit = entity.CurrentUnit,
            PerUnitRate = entity.PerUnitRate,
            TotalUnit = entity.CurrentUnit - entity.PreviousUnit,
            TotalAmount = (entity.CurrentUnit - entity.PreviousUnit) * entity.PerUnitRate
        };
    }

    // =====================================================
    // GET ALL  ✅ THIS FIXES YOUR ERROR
    // =====================================================
    public async Task<List<ElectricBillVm>> GetAllAsync()
    {
        return await context.Set<ElectricBill>()
            .Include(x => x.MealCycle)   // 🔴 REQUIRED
            .OrderByDescending(x => x.Id)
            .Select(x => new ElectricBillVm
            {
                Id = x.Id,
                MealCycleId = x.MealCycleId,           

                PreviousUnit = x.PreviousUnit,
                CurrentUnit = x.CurrentUnit,
                PerUnitRate = x.PerUnitRate,
                TotalUnit = x.CurrentUnit - x.PreviousUnit,
                TotalAmount = (x.CurrentUnit - x.PreviousUnit) * x.PerUnitRate
            })
            .ToListAsync();
    }

    // =====================================================
    // DELETE
    // =====================================================
    public async Task<bool> DeleteAsync(long id)
    {
        var entity = await context.Set<ElectricBill>()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (entity == null)
            return false;

        context.Set<ElectricBill>().Remove(entity);
        await context.SaveChangesAsync();
        return true;
    }
}
