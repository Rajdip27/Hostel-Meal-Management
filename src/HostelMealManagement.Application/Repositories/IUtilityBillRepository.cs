using HostelMealManagement.Application.Repositories.Base;
using HostelMealManagement.Application.ViewModel;
using HostelMealManagement.Core.Entities;
using HostelMealManagement.Infrastructure.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace HostelMealManagement.Application.Repositories;

#region INTERFACE

public interface IUtilityBillRepository : IBaseService<UtilityBill>
{
    Task<bool> UpsertAsync(UtilityBillVm vm);
    Task<UtilityBillVm?> GetByIdAsync(long id);
    Task<List<UtilityBillVm>> GetAllAsync();
    Task<bool> DeleteAsync(long id);
}

#endregion

#region IMPLEMENTATION

public class UtilityBillRepository : BaseService<UtilityBill>, IUtilityBillRepository
{
    private readonly ApplicationDbContext context;

    public UtilityBillRepository(ApplicationDbContext context)
        : base(context)
    {
        this.context = context;
    }

    // ============================
    // CREATE / UPDATE
    // ============================
    public async Task<bool> UpsertAsync(UtilityBillVm vm)
    {
        using var trx = await context.Database.BeginTransactionAsync();
        try
        {
            var isNew = vm.Id == 0;

            // Prevent duplicate utility bill for same month
            var exists = await context.Set<UtilityBill>()
                .AnyAsync(x =>
                    x.Id != vm.Id &&
                    x.UtilityType == vm.UtilityType &&
                    x.BillYear == vm.BillYear &&
                    x.BillMonth == vm.BillMonth);

            if (exists)
                return false;

            var entity = isNew
                ? new UtilityBill()
                : await context.Set<UtilityBill>()
                    .FirstOrDefaultAsync(x => x.Id == vm.Id);

            if (entity == null)
                return false;

            // ============================
            // MASTER DATA
            // ============================
            entity.UtilityType = vm.UtilityType;
            entity.BillYear = vm.BillYear;
            entity.BillMonth = vm.BillMonth;
            entity.BillDate = vm.BillDate;

            entity.CurrentUnit = vm.CurrentUnit;
            entity.PerUnitRate = vm.PerUnitRate;

            // ============================
            // CALCULATION
            // ============================
            if (vm.UtilityType == UtilityType.Electric)
            {
                var unit = vm.CurrentUnit ?? 0;
                var rate = vm.PerUnitRate ?? 0;

                entity.TotalUnit = unit;
                entity.TotalAmount = unit * rate;
            }
            else
            {
                // Water / Gas / Servant
                entity.TotalUnit = 0;
                entity.TotalAmount = vm.TotalAmount;
            }

            if (isNew)
                context.Set<UtilityBill>().Add(entity);

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

    // ============================
    // GET BY ID
    // ============================
    public async Task<UtilityBillVm?> GetByIdAsync(long id)
    {
        var entity = await context.Set<UtilityBill>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (entity == null)
            return null;

        return new UtilityBillVm
        {
            Id = entity.Id,
            UtilityType = entity.UtilityType,

            BillYear = entity.BillYear,
            BillMonth = entity.BillMonth,
            BillDate = entity.BillDate,

            CurrentUnit = entity.CurrentUnit,
            PerUnitRate = entity.PerUnitRate,
            TotalUnit = entity.TotalUnit,
            TotalAmount = entity.TotalAmount
        };
    }

    // ============================
    // GET ALL
    // ============================
    public async Task<List<UtilityBillVm>> GetAllAsync()
    {
        return await context.Set<UtilityBill>()   // ✅ ENTITY
            .AsNoTracking()
            .OrderByDescending(x => x.BillYear)
            .ThenByDescending(x => x.BillMonth)
            .ThenByDescending(x => x.Id)
            .Select(entity => new UtilityBillVm   // ✅ VM
            {
                Id = entity.Id,
                UtilityType = entity.UtilityType,

                BillYear = entity.BillYear,
                BillMonth = entity.BillMonth,
                BillDate = entity.BillDate,

                CurrentUnit = entity.CurrentUnit,
                PerUnitRate = entity.PerUnitRate,
                TotalUnit = entity.TotalUnit,
                TotalAmount = entity.TotalAmount
            })
            .ToListAsync();
    }


    // ============================
    // DELETE
    // ============================
    public async Task<bool> DeleteAsync(long id)
    {
        using var trx = await context.Database.BeginTransactionAsync();
        try
        {
            var entity = await context.Set<UtilityBill>()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return false;

            context.Set<UtilityBill>().Remove(entity);

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
}

#endregion
