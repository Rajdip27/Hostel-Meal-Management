using HostelMealManagement.Application.Repositories.Base;
using HostelMealManagement.Application.ViewModel.SSLCommerz;
using HostelMealManagement.Core.Entities;
using HostelMealManagement.Infrastructure.DatabaseContext;

namespace HostelMealManagement.Application.Repositories;

public interface IPaymentTransactionRepository:IBaseService<PaymentTransaction>
{
    Task<SSLPaymentRequestViewModel> CreateSSLPaymentRequestViewModelAsync(long memberId, long mealCycleId,long billId);
}

public class PaymentTransactionRepository(ApplicationDbContext context,  IMealBillRepository mealBillRepository) : BaseService<PaymentTransaction>(context), IPaymentTransactionRepository
{
    public async Task<SSLPaymentRequestViewModel> CreateSSLPaymentRequestViewModelAsync(
     long memberId,
     long mealCycleId,
     long billId)
    {
        try
        {
            var bill = await mealBillRepository.FindAsync(
                x => x.Id == billId,
                x => x.Member,
                x => x.MealCycle
            );

            if (bill is null)
                return null;
            return new SSLPaymentRequestViewModel
            {
                Amount = bill.NetPayable,
                CustomerName = bill.Member?.Name ?? string.Empty,
                CustomerEmail = bill.Member?.Email ?? string.Empty,
                CustomerPhone = bill.Member?.PhoneNumber ?? string.Empty,
                ProductName = bill.MealCycle?.Name ?? "Meal Bill"
            };
        }
        catch (Exception)
        {
            return null;
        }
    }

}
