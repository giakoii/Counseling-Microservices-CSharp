using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using AppointmentService.Application.Appointments.Commands;
using AppointmentService.Application.Appointments.Commands.InsertAppointment;
using AppointmentService.Application.PaymentServices;
using Common.Utils.Const;

namespace AppointmentService.Infrastructure.PaymentServices;

public class PaymentService : IPaymentService
{
    public async Task<AppointmentInsertResponse> PaymentAppointment(Guid appointmentId)
    {
        var response = new AppointmentInsertResponse { Success = false };

        // Tạo request cho PayOS
        var orderCode = DateTimeOffset.UtcNow.ToUnixTimeSeconds(); 
        
        // Limit description to 25 characters
        var description = $"Thanh toán cho hoá đơn: {appointmentId}";
        if (description.Length > 25)
            description = description.Substring(0, 25);    
        
        DotNetEnv.Env.Load(); 
        
        var payOsCheckSumKey = Environment.GetEnvironmentVariable(ConstEnv.PayOsCheckSumKey);
        var payOsApiKey = Environment.GetEnvironmentVariable(ConstEnv.PayOsApiKey);
        var payOsClientId = Environment.GetEnvironmentVariable(ConstEnv.PayOsClientId);
        
        var returnUrl = $"http://localhost:3000/payment?appointmentId={appointmentId}";
        var cancelUrl = $"http://localhost:3000/payment-cancel?appointmentId={appointmentId}";
        var data = $"amount={2000}&cancelUrl={cancelUrl}" +
                   $"&description={description}" +
                   $"&orderCode={orderCode}" +
                   $"&returnUrl={returnUrl}";
        string signature = ComputeHmacSha256(data, payOsCheckSumKey!);

        var payRequest = new
        {
            orderCode = orderCode,
            amount = 2000,
            description = description,
            returnUrl = returnUrl,
            cancelUrl = cancelUrl,
            signature = signature,
        };
        
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("x-client-id", payOsClientId);
        client.DefaultRequestHeaders.Add("x-api-key", payOsApiKey);

        var jsonContent = new StringContent(JsonSerializer.Serialize(payRequest), Encoding.UTF8, "application/json");

        var payosResponse = await client.PostAsync("https://api-merchant.payos.vn/v2/payment-requests", jsonContent);

        if (!payosResponse.IsSuccessStatusCode)
        {
            response.SetMessage(MessageId.E00000, "Failed to create payment request");
            return response;
        }

        var responseContent = await payosResponse.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(responseContent);
        var root = jsonDoc.RootElement;

        var dataElement = root.GetProperty("data");

        var checkoutUrl = dataElement.GetProperty("checkoutUrl").GetString();
        var qrCode = dataElement.GetProperty("qrCode").GetString();
        if (string.IsNullOrEmpty(checkoutUrl) || string.IsNullOrEmpty(qrCode))
        {
            response.SetMessage(MessageId.E00000, "Failed to retrieve payment data");
            return response;
        }

        var entityResponse = new AppointmentInsertEntity
        {
            CheckoutUrl = checkoutUrl,
            QrCode = qrCode,
        };
        
        response.Success = true;
        response.Response = entityResponse;
        response.AppointmentId = appointmentId.ToString();
        response.TransactionId = orderCode.ToString();
        return response;
    }
    
    public static string ComputeHmacSha256(string message, string secretKey)
    {
        if (string.IsNullOrWhiteSpace(message)) throw new ArgumentNullException(nameof(message));
        if (string.IsNullOrWhiteSpace(secretKey)) throw new ArgumentNullException(nameof(secretKey));

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
        byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));

        // Convert to lowercase hex string without hyphens
        var hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        return hashString;
    }
}