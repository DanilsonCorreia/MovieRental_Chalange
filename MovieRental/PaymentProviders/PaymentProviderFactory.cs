using Microsoft.Extensions.DependencyInjection;

namespace MovieRental.PaymentProviders
{
    public interface IPaymentProviderFactory
    {
        IPaymentProvider GetPaymentProvider(string paymentMethod);
        IEnumerable<string> GetAvailablePaymentMethods();
    }

    public class PaymentProviderFactory : IPaymentProviderFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<string, Type> _paymentProviders;

        public PaymentProviderFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _paymentProviders = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                { "PayPal", typeof(PayPalProvider) },
                { "MbWay", typeof(MbWayProvider) }
            };
        }

        public IPaymentProvider GetPaymentProvider(string paymentMethod)
        {
            if (string.IsNullOrWhiteSpace(paymentMethod))
                throw new ArgumentException("Payment method cannot be null or empty.");

            if (!_paymentProviders.ContainsKey(paymentMethod))
                throw new ArgumentException($"Payment method '{paymentMethod}' is not supported.");

            var providerType = _paymentProviders[paymentMethod];
            return (IPaymentProvider)ActivatorUtilities.CreateInstance(_serviceProvider, providerType);
        }

        public IEnumerable<string> GetAvailablePaymentMethods()
        {
            return _paymentProviders.Keys;
        }
    }
} 