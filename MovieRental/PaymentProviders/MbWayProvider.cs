namespace MovieRental.PaymentProviders
{
    public class MbWayProvider : IPaymentProvider
    {
        public string ProviderName => "MbWay";

        public async Task<bool> ProcessPaymentAsync(double amount)
        {
            
            await Task.Delay(150);

            return true;
        }
    }
}
