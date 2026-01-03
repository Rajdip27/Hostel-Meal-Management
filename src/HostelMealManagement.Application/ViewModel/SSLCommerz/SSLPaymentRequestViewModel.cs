namespace HostelMealManagement.Application.ViewModel.SSLCommerz;

public class SSLPaymentRequestViewModel
{
    public decimal Amount { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
}
