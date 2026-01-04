namespace HostelMealManagement.Application.ViewModel.SSLCommerz;

public class PaymentResult
{
    public string TransactionId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; }
    public string Message { get; set; }
    public string CustomerName { get; set; }
    public long MealBillId { get; set; }
    public long MealCycleId { get; set; }
}
