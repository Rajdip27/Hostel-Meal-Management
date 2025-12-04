using HostelMealManagement.Application.Repositories.Base;
using HostelMealManagement.Application.ViewModel;
using HostelMealManagement.Core.Entities;
using HostelMealManagement.Infrastructure.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace HostelMealManagement.Application.Repositories;

public interface IMealBazarRepository:IBaseService<MealBazar>
{
    Task<bool> UpsertAsync(MealBazarVm vm);
    Task<MealBazarVm> GetByIdAsync(long id);
    Task<List<MealBazarVm>> GetAllAsync();
    Task<bool> DeleteAsync(long id);
}
public class MealBazarRepository(ApplicationDbContext context) : BaseService<MealBazar>(context), IMealBazarRepository
{
    public async Task<bool> UpsertAsync(MealBazarVm vm)
    {
        using var trx = await context.Database.BeginTransactionAsync();

        try
        {
            MealBazar bazar;

            if (vm.Id == 0)
            {
                // INSERT
                bazar = new MealBazar
                {
                    BazarDate = vm.BazarDate,
                    BazarAmount = vm.BazarAmount,
                    Description = vm.Description
                };

                context.Set<MealBazar>().Add(bazar);
                await context.SaveChangesAsync();
            }
            else
            {
                // UPDATE
                bazar = await context.Set<MealBazar>()
                    .Include(x => x.Items)
                    .FirstOrDefaultAsync(x => x.Id == vm.Id);

                if (bazar == null) return false; // Record not found

                bazar.BazarDate = vm.BazarDate;
                bazar.BazarAmount = vm.BazarAmount;
                bazar.Description = vm.Description;

                context.Set<MealBazar>().Update(bazar);

                // Remove old items
                context.Set<MealBazarItem>().RemoveRange(bazar.Items);
            }

            // Insert items
            var items = vm.Items.Select(itemVm => new MealBazarItem
            {
                MealBazarId = bazar.Id,
                ProductName = itemVm.ProductName,
                Quantity = itemVm.Quantity,
                Price = itemVm.Price
            }).ToList();

            // Add all at once
            context.Set<MealBazarItem>().AddRange(items);

            await context.SaveChangesAsync();
            await trx.CommitAsync();

            return true; // Success
        }
        catch
        {
            await trx.RollbackAsync();
            return false; // Failed
        }
    }

    /// <summary>
    /// Get MealBazar by Id including items
    /// </summary>
    public async Task<MealBazarVm> GetByIdAsync(long id)
    {
        var entity = await context.Set<MealBazarVm>()
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (entity == null) return null;

        return new MealBazarVm
        {
            Id = entity.Id,
            BazarDate = entity.BazarDate,
            BazarAmount = entity.BazarAmount,
            Description = entity.Description,
            Items = entity.Items.Select(i => new MealBazarItemVm
            {
                Id = i.Id,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                Price = i.Price
            }).ToList()
        };
    }

    public async Task<bool> DeleteAsync(long id)
    {
        using var trx = await context.Database.BeginTransactionAsync();

        try
        {
            var entity = await context.Set<MealBazar>()
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null) return false;

            context.Set<MealBazarItem>().RemoveRange(entity.Items);
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

    public async Task<List<MealBazarVm>> GetAllAsync()
    {
        return await context.Set<MealBazar>()
            .Include(x => x.Items)
            .OrderByDescending(x => x.BazarDate)
            .Select(entity => new MealBazarVm
            {
                Id = entity.Id,
                BazarDate = entity.BazarDate,
                BazarAmount = entity.BazarAmount,
                Description = entity.Description,
                Items = entity.Items.Select(i => new MealBazarItemVm
                {
                    Id = i.Id,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList()
            }).ToListAsync();
    }
}
