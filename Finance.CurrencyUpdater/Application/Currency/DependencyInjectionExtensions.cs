namespace Finance.CurrencyUpdater.Application.Currency;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddCurrencyServices(this IServiceCollection self)
    {
        self.AddHttpClient<CbrCurrencyProvider>(client =>
        {
            client.BaseAddress = new Uri("https://www.cbr.ru/scripts/XML_daily.asp");
        });
        self.AddSingleton<ICurrencyProvider, CbrCurrencyProvider>();
        self.AddSingleton<ICurrencyUpdater, CurrencyUpdater>();
        return self;
    }
}