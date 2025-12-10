using HostelMealManagement.Application.Repositories.Base;
using HostelMealManagement.Core.Entities;
using HostelMealManagement.Infrastructure.DatabaseContext;

namespace HostelMealManagement.Application.Repositories;

public interface IMealMenuRepository : IBaseService<MealMenu>
{
}

public class MealMenuRepository(ApplicationDbContext context)
    : BaseService<MealMenu>(context), IMealMenuRepository
{
}

