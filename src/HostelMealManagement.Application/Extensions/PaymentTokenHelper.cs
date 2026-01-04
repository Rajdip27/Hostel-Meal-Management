using HostelMealManagement.Application.CommonModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HostelMealManagement.Application.Extensions;

public static class PaymentTokenHelper
{
    private static readonly string Secret = "SUPER_SECRET_KEY_123";

    public static string Generate(PaymentPayload payload)
    {
        var json = JsonSerializer.Serialize(payload);
        var data = System.Text.Encoding.UTF8.GetBytes(json);

        using var hmac = new System.Security.Cryptography.HMACSHA256(
            System.Text.Encoding.UTF8.GetBytes(Secret));

        var signature = hmac.ComputeHash(data);

        return Convert.ToBase64String(data) + "." +
               Convert.ToBase64String(signature);
    }

    public static PaymentPayload Validate(string token)
    {
        var parts = token.Split('.');
        if (parts.Length != 2)
            return null;

        var data = Convert.FromBase64String(parts[0]);
        var signature = Convert.FromBase64String(parts[1]);

        using var hmac = new System.Security.Cryptography.HMACSHA256(
            System.Text.Encoding.UTF8.GetBytes(Secret));

        var computed = hmac.ComputeHash(data);

        if (!computed.SequenceEqual(signature))
            return null;

        var payload = JsonSerializer.Deserialize<PaymentPayload>(
            System.Text.Encoding.UTF8.GetString(data));

        if (payload.ExpireAt < DateTime.UtcNow)
            return null;

        return payload;
    }
}
