namespace HostelMealManagement.Application.ViewModel.SSLCommerz;

public class PaymentSuccessViewModel
{
    public string TransactionId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; }
    public string Message { get; set; }
    public DateTime PaymentDate { get; set; }
    public string CustomerName { get; set; }
}
