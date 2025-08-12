namespace Finance.CurrencyUpdater.Application.Currency;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddCurrencyServices(this IServiceCollection self)
    {
        self.AddHttpClient<CbrCurrencyProvider>();
        self.AddSingleton<ICurrencyProvider, CbrCurrencyProvider>();
        self.AddSingleton<ICurrencyUpdater, CurrencyUpdater>();
        return self;
    }
}