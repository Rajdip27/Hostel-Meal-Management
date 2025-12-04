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
                // CREATE new master with children
                bazar = new MealBazar
                {
                    BazarDate = vm.BazarDate,
                    BazarAmount = vm.BazarAmount,
                    Description = vm.Description,
                    Items = vm.Items?.Select(i => new MealBazarItem
                    {
                        ProductName = i.ProductName,
                        Quantity = i.Quantity,
                        Price = i.Price
                    }).ToList() ?? new List<MealBazarItem>()
                };

                context.Set<MealBazar>().Add(bazar);
                await context.SaveChangesAsync(); // obtains bazar.Id
                await trx.CommitAsync();
                return true;
            }

            // ===== UPDATE =====
            // Load tracked entity including items
            bazar = await context.Set<MealBazar>()
                .Include(b => b.Items)
                .FirstOrDefaultAsync(b => b.Id == vm.Id);

            if (bazar == null) return false;

            // Update master fields
            bazar.BazarDate = vm.BazarDate;
            bazar.BazarAmount = vm.BazarAmount;
            bazar.Description = vm.Description;

            // Make dictionary of DB items for quick lookup
            var dbItemsById = bazar.Items.ToDictionary(x => x.Id);

            // IDs sent from UI (existing ones)
            var vmExistingIds = vm.Items?.Where(x => x.Id > 0).Select(x => x.Id).ToHashSet()
                                ?? new HashSet<long>();

            // 1) Update existing and add new
            foreach (var itemVm in vm.Items ?? Enumerable.Empty<MealBazarItemVm>())
            {
                if (itemVm.Id > 0)
                {
                    // update existing
                    if (dbItemsById.TryGetValue(itemVm.Id, out var dbItem))
                    {
                        dbItem.ProductName = itemVm.ProductName;
                        dbItem.Quantity = itemVm.Quantity;
                        dbItem.Price = itemVm.Price;
                        // no explicit Update() required — it's tracked
                        dbItemsById.Remove(itemVm.Id); // mark as processed
                    }
                    else
                    {
                        // edge: UI sent an Id but not present in DB -> treat as add
                        var newItem = new MealBazarItem
                        {
                            MealBazarId = bazar.Id,
                            ProductName = itemVm.ProductName,
                            Quantity = itemVm.Quantity,
                            Price = itemVm.Price
                        };
                        bazar.Items.Add(newItem);
                    }
                }
                else
                {
                    // new item
                    var newItem = new MealBazarItem
                    {
                        MealBazarId = bazar.Id,
                        ProductName = itemVm.ProductName,
                        Quantity = itemVm.Quantity,
                        Price = itemVm.Price
                    };
                    bazar.Items.Add(newItem);
                }
            }

            // 2) Delete DB items that were not present in UI (remaining in dbItemsById)
            if (dbItemsById.Any())
            {
                context.Set<MealBazarItem>().RemoveRange(dbItemsById.Values);
            }

            // Save once for all changes
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
