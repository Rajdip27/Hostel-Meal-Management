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
            MealBazar bazar;

            // ================= CREATE =================
            if (vm.Id == 0)
            {
                bazar = new MealBazar
                {
                    BazarDate = vm.BazarDate,
                    StartDate = vm.StartDate,
                    EndDate = vm.EndDate,
                    TotalDays = vm.TotalDays,
                    MealMemberId = vm.MealMemberId, // "1,2,3"
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
                await context.SaveChangesAsync();
                await trx.CommitAsync();
                return true;
            }

            // ================= UPDATE =================
            bazar = await context.Set<MealBazar>()
                .Include(b => b.Items)
                .FirstOrDefaultAsync(b => b.Id == vm.Id);

            if (bazar == null) return false;

            // Update master
            bazar.BazarDate = vm.BazarDate;
            bazar.StartDate = vm.StartDate;
            bazar.EndDate = vm.EndDate;
            bazar.TotalDays = vm.TotalDays;
            bazar.MealMemberId = vm.MealMemberId;
            bazar.BazarAmount = vm.BazarAmount;
            bazar.Description = vm.Description;

            // ---------- CHILD SYNC ----------
            var dbItemsById = bazar.Items.ToDictionary(x => x.Id);

            foreach (var itemVm in vm.Items ?? Enumerable.Empty<MealBazarItemVm>())
            {
                if (itemVm.Id > 0 && dbItemsById.TryGetValue(itemVm.Id, out var dbItem))
                {
                    // update
                    dbItem.ProductName = itemVm.ProductName;
                    dbItem.Quantity = itemVm.Quantity;
                    dbItem.Price = itemVm.Price;

                    dbItemsById.Remove(itemVm.Id);
                }
                else
                {
                    // add new
                    bazar.Items.Add(new MealBazarItem
                    {
                        MealBazarId = bazar.Id,
                        ProductName = itemVm.ProductName,
                        Quantity = itemVm.Quantity,
                        Price = itemVm.Price
                    });
                }
            }

            // delete removed items
            if (dbItemsById.Any())
            {
                context.Set<MealBazarItem>().RemoveRange(dbItemsById.Values);
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

    // ----------------------------------------------------------------------
    // GET BY ID
    // ----------------------------------------------------------------------
    public async Task<MealBazarVm?> GetByIdAsync(long id)
    {
        var entity = await context.Set<MealBazar>()
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (entity == null) return null;

        return new MealBazarVm
        {
            Id = entity.Id,
            BazarDate = entity.BazarDate,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            TotalDays = entity.TotalDays,
            MealMemberId = entity.MealMemberId,
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

    // ----------------------------------------------------------------------
    // DELETE
    // ----------------------------------------------------------------------
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

    // ----------------------------------------------------------------------
    // GET ALL
    // ----------------------------------------------------------------------
    public async Task<List<MealBazarVm>> GetAllAsync()
    {
        return await context.Set<MealBazar>()
            .Include(x => x.Items)
            .OrderByDescending(x => x.BazarDate)
            .Select(entity => new MealBazarVm
            {
                Id = entity.Id,
                BazarDate = entity.BazarDate,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                TotalDays = entity.TotalDays,
                MealMemberId = entity.MealMemberId,
                BazarAmount = entity.BazarAmount,
                Description = entity.Description,
                Items = entity.Items.Select(i => new MealBazarItemVm
                {
                    Id = i.Id,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList()
            })
            .ToListAsync();
    }
}

#endregion
