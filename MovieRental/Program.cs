using MovieRental.Data;
using MovieRental.Movie;
using MovieRental.Rental;
using MovieRental.Customer;
using MovieRental.Middleware;
using NLog.Web;
using MovieRental.PaymentProviders;
using MovieRental.Payment;

var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
try
{
    var builder = WebApplication.CreateBuilder(args);


    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddEntityFrameworkSqlite().AddDbContext<MovieRentalDbContext>();

    builder.Services.AddScoped<IRentalFeatures, RentalFeatures>();
    builder.Services.AddScoped<ICustomerFeatures, CustomerFeatures>();
    builder.Services.AddScoped<IPaymentProviderFactory, PaymentProviderFactory>();
    builder.Services.AddScoped<IPaymentFeature, PaymentFeature>();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();


    app.UseMiddleware<GlobalExceptionHandler>();

    app.UseAuthorization();

    app.MapControllers();

    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<MovieRentalDbContext>();
        dbContext.Database.EnsureCreated();
    }

    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex, "Stopped program because of exception");
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}
