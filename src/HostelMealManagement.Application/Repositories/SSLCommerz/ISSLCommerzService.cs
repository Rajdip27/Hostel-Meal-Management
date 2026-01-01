using HostelMealManagement.Application.ViewModel.SSLCommerz;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace HostelMealManagement.Application.Repositories.SSLCommerz;

public interface ISSLCommerzService
{
    Task<string> CreatePaymentAsync(SSLPaymentRequest request);
    Task<string> ValidateAsync(string valId);
}
public sealed class SSLCommerzService : ISSLCommerzService
{
    private readonly SSLCommerzOptions _opt;
    private readonly HttpClient _http;

    public SSLCommerzService(IOptions<SSLCommerzOptions> opt)
    {
        _opt = opt.Value;
        _http = new HttpClient();
    }

    public async Task<string> CreatePaymentAsync(SSLPaymentRequest r)
    {
        var data = new Dictionary<string, string>
        {
            ["store_id"] = _opt.StoreId,
            ["store_passwd"] = _opt.StorePassword,
            ["total_amount"] = r.Amount.ToString(),
            ["currency"] = "BDT",
            ["tran_id"] = r.TransactionId,

            ["success_url"] = r.SuccessUrl,
            ["fail_url"] = r.FailUrl,
            ["cancel_url"] = r.CancelUrl,

            ["cus_name"] = r.CustomerName,
            ["cus_email"] = r.CustomerEmail,
            ["cus_phone"] = r.CustomerPhone,

            ["product_name"] = r.ProductName,
            ["product_profile"] = "general"
        };

        var response = await _http.PostAsync(
            $"{_opt.BaseUrl}/gwprocess/v3/api.php",
            new FormUrlEncodedContent(data));

        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);

        if (doc.RootElement.GetProperty("status").GetString() != "SUCCESS")
            throw new Exception("SSLCommerz payment failed");

        return doc.RootElement.GetProperty("GatewayPageURL").GetString();
    }

    public Task<string> ValidateAsync(string valId)
    {
        var url =
            $"{_opt.BaseUrl}/validator/api/validationserverAPI.php" +
            $"?val_id={valId}&store_id={_opt.StoreId}" +
            $"&store_passwd={_opt.StorePassword}&format=json";

        return _http.GetStringAsync(url);
    }
}
