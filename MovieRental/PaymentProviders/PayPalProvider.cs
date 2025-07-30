namespace MovieRental.PaymentProviders
{
    public class PayPalProvider : IPaymentProvider
    {
        public string ProviderName => "PayPal";

        public async Task<bool> ProcessPaymentAsync(double amount)
        {
            // Simulate PayPal payment processing
            await Task.Delay(100); // Simulate network delay
            
            // For demo purposes, let's say PayPal always succeeds
            // In a real implementation, this would call PayPal's API
            return true;
        }
    }
}
