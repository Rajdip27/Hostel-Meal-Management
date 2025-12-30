using HostelMealManagement.Application.Repositories.Base;
using HostelMealManagement.Core.Entities;
using HostelMealManagement.Infrastructure.DatabaseContext;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HostelMealManagement.Application.Repositories;

public interface INormalPaymentRepository : IBaseService<NormalPayment>
{
    List<SelectListItem> GetNormalPaymentList();
}

public class NormalPaymentRepository : BaseService<NormalPayment>, INormalPaymentRepository
{
    public NormalPaymentRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public List<SelectListItem> GetNormalPaymentList()
    {
        return _context.Set<NormalPayment>()
            .Where(x => !x.IsDelete)
            .Include(x => x.Member)
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.Member.Name} - {x.PaymentDate:dd MMM yyyy} - {x.PaymentAmount}"
            })
            .ToList();
    }
}
