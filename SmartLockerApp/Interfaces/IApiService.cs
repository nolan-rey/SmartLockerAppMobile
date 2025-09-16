using SmartLockerApp.Models;

namespace SmartLockerApp.Interfaces;

public interface IApiService
{
    // Authentication
    Task<ApiResponse<User>> LoginAsync(string email, string password);
    Task<ApiResponse<User>> RegisterAsync(string firstName, string lastName, string email, string password);
    Task<ApiResponse<bool>> LogoutAsync();

    // Lockers
    Task<ApiResponse<List<Locker>>> GetAvailableLockersAsync();
    Task<ApiResponse<Locker>> GetLockerByIdAsync(string lockerId);
    Task<ApiResponse<bool>> ReserveLockerAsync(string lockerId);

    // Sessions
    Task<ApiResponse<LockerSession>> CreateSessionAsync(string lockerId, int durationHours, List<string> items);
    Task<ApiResponse<LockerSession>> GetSessionAsync(string sessionId);
    Task<ApiResponse<List<LockerSession>>> GetUserSessionsAsync(string userId);
    Task<ApiResponse<bool>> EndSessionAsync(string sessionId);
    Task<ApiResponse<bool>> ExtendSessionAsync(string sessionId, int additionalHours);

    // Locker Control
    Task<ApiResponse<bool>> UnlockLockerAsync(string lockerId, string method);
    Task<ApiResponse<bool>> LockLockerAsync(string lockerId);
    Task<ApiResponse<string>> GetLockerStatusAsync(string lockerId);

    // Payments
    Task<ApiResponse<decimal>> CalculatePriceAsync(double hours);
    Task<ApiResponse<bool>> ProcessPaymentAsync(string sessionId, string paymentMethod, decimal amount);
    Task<ApiResponse<List<PaymentRecord>>> GetPaymentHistoryAsync(string userId);
}

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public string? ErrorCode { get; set; }

    public static ApiResponse<T> SuccessResult(T data) => new()
    {
        Success = true,
        Data = data
    };

    public static ApiResponse<T> ErrorResult(string message, string? errorCode = null) => new()
    {
        Success = false,
        Message = message,
        ErrorCode = errorCode
    };
}

public class PaymentRecord
{
    public string Id { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
    public string Status { get; set; } = string.Empty;
}
