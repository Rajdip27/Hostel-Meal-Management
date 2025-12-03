using HostelMealManagement.Application.Repositories.Base;
using HostelMealManagement.Core.Entities;
using HostelMealManagement.Infrastructure.DatabaseContext;

namespace HostelMealManagement.Application.Repositories;

public interface IMealBazarRepository:IBaseService<MealBazar>
{
}
public class MealBazarRepository : BaseService<MealBazar>, IMealBazarRepository
{
    public MealBazarRepository(ApplicationDbContext context) : base(context)
    {
    }
}
