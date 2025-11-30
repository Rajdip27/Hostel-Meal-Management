using HostelMealManagement.Application.Repositories.Base;
using HostelMealManagement.Core.Entities;
using HostelMealManagement.Infrastructure.DatabaseContext;

namespace HostelMealManagement.Application.Repositories;

public interface IMealCycleRepository:IBaseService<MealCycle>
{
}

public class MealCycleRepository(ApplicationDbContext context) : BaseService<MealCycle>(context), IMealCycleRepository
{
}
