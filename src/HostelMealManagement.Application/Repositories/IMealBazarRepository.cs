using HostelMealManagement.Application.Extensions;
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
    public async Task<bool> UpsertAsync(MealBazarVm vm)
    {
        using var trx = await context.Database.BeginTransactionAsync();
        try
        {
            var isNew = vm.Id == 0;

            var bazar = isNew
                ? new MealBazar()
                : await context.Set<MealBazar>()
                    .Include(x => x.Items)
                    .FirstOrDefaultAsync(x => x.Id == vm.Id);

            if (bazar == null) return false;

            // ===== MASTER =====
            bazar.BazarDate = vm.BazarDate;
            bazar.BazarAmount = vm.BazarAmount;
            bazar.Description = vm.Description;
            bazar.StartDate = vm.StartDate;
            bazar.EndDate = vm.EndDate;
            bazar.TotalDays = vm.TotalDays;
            bazar.MealMemberId = vm.MemberIds;

            // ===== ITEMS =====
            var dbItems = bazar.Items?.ToDictionary(x => x.Id) ?? new();

            foreach (var item in vm.Items ?? Enumerable.Empty<MealBazarItemVm>())
            {
                if (item.Id > 0 && dbItems.TryGetValue(item.Id, out var dbItem))
                {
                    dbItem.ProductName = item.ProductName;
                    dbItem.Quantity = item.Quantity;
                    dbItem.Price = item.Price;
                    dbItems.Remove(item.Id);
                }
                else
                {
                    bazar.Items ??= new();
                    bazar.Items.Add(new MealBazarItem
                    {
                        ProductName = item.ProductName,
                        Quantity = item.Quantity,
                        Price = item.Price
                    });
                }
            }

            // delete removed items
            if (dbItems.Any())
                context.RemoveRange(dbItems.Values);

            if (isNew)
                context.Add(bazar);

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



    /// <summary>
    /// Get MealBazar by Id including items
    /// </summary>
    public async Task<MealBazarVm> GetByIdAsync(long id)
    {
        var entity = await context.Set<MealBazar>()
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (entity == null) return null;

        return new MealBazarVm
        {
            Id = entity.Id,
            BazarDate = entity.BazarDate,
            BazarAmount = entity.BazarAmount,
            Description = entity.Description,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            TotalDays = entity.TotalDays,
            MemberIds = entity.MealMemberId,
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

#endregion
