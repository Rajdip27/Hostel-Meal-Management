using HostelMealManagement.Application.Repositories.Base;
using HostelMealManagement.Application.ViewModel;
using HostelMealManagement.Core.Entities;
using HostelMealManagement.Infrastructure.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace HostelMealManagement.Application.Repositories;

#region INTERFACE

public interface IMealBazarRepository : IBaseService<MealBazar>
{
    Task<bool> UpsertAsync(MealBazarVm vm);
    Task<MealBazarVm?> GetByIdAsync(long id);
    Task<List<MealBazarVm>> GetAllAsync();
    Task<bool> DeleteAsync(long id);
}

#endregion

#region IMPLEMENTATION

public class MealBazarRepository(ApplicationDbContext context)
    : BaseService<MealBazar>(context), IMealBazarRepository
{
    // ----------------------------------------------------------------------
    // UPSERT
    // ----------------------------------------------------------------------
    public async Task<bool> UpsertAsync(MealBazarVm vm)
    {
        using var trx = await context.Database.BeginTransactionAsync();

        try
        {
            MealBazar entity;

            // ========== CREATE ==========
            if (vm.Id == 0)
            {
                entity = new MealBazar
                {
                    BazarDate = vm.BazarDate,
                    StartDate = vm.StartDate,
                    EndDate = vm.EndDate,
                    TotalDays = vm.TotalDays,
                    MealMemberId = vm.MemberIds,
                    BazarAmount = vm.BazarAmount,
                    Description = vm.Description
                };

                context.Set<MealBazar>().Add(entity);
                await context.SaveChangesAsync();
                await trx.CommitAsync();
                return true;
            }

            // ========== UPDATE ==========
            entity = await context.Set<MealBazar>()
                .FirstOrDefaultAsync(x => x.Id == vm.Id);

            if (entity == null)
                return false;

            entity.BazarDate = vm.BazarDate;
            entity.StartDate = vm.StartDate;
            entity.EndDate = vm.EndDate;
            entity.TotalDays = vm.TotalDays;
            entity.MealMemberId = vm.MemberIds;
            entity.BazarAmount = vm.BazarAmount;
            entity.Description = vm.Description;

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
    public async Task<MealBazarVm?> GetByIdAsync(long id)
    {
        var entity = await context.Set<MealBazar>()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (entity == null)
            return null;

        return new MealBazarVm
        {
            Id = entity.Id,
            BazarDate = entity.BazarDate,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            
            //MealMemberId = entity.MealMemberId,
            BazarAmount = entity.BazarAmount,
            Description = entity.Description
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
            var entity = await context.Set<MealBazar>()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return false;

            context.Set<MealBazar>().Remove(entity);
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
    public async Task<List<MealBazarVm>> GetAllAsync()
    {
        return await context.Set<MealBazar>()
            .OrderByDescending(x => x.BazarDate)
            .Select(x => new MealBazarVm
            {
                Id = x.Id,
                BazarDate = x.BazarDate,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                //TotalDays = x.TotalDays,
                //MealMemberId = x.MealMemberId,
                BazarAmount = x.BazarAmount,
                Description = x.Description
            })
            .ToListAsync();
    }



}

#endregion
