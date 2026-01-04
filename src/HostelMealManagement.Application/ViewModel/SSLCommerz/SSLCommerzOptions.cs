namespace HostelMealManagement.Application.ViewModel.SSLCommerz;

public class SSLCommerzOptions
{
    public string StoreId { get; set; }
    public string StorePassword { get; set; }
    public string BaseUrl { get; set; }
}

public record SSLPaymentRequest(
    decimal Amount,
    string TransactionId,
    string SuccessUrl,
    string FailUrl,
    string CancelUrl,
    string CustomerName,
    string CustomerEmail,
    string CustomerPhone,
    string ProductName,
    string value_a,
    string value_b
);

