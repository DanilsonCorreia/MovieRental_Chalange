namespace MovieRental.PaymentProviders
{
    public interface IPaymentProvider
    {
        string ProviderName { get; }
        Task<bool> ProcessPaymentAsync(double amount);
    }
} 