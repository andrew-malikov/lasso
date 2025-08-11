using Finance.Db;
using Microsoft.EntityFrameworkCore;

namespace Finance.CurrencyUpdater.Application.Currency;

public class CurrencyUpdater(FinanceDbContext dbContext, ICurrencyProvider currencyProvider) : ICurrencyUpdater
{
    public async Task Update(CancellationToken token)
    {
        var storedTask = dbContext.Currencies.ToDictionaryAsync(c => c.Id, token);
        var externalTask = currencyProvider.GetAll(token);

        await Task.WhenAll(storedTask, externalTask);

        var stored = storedTask.Result;
        var external = externalTask.Result;

        foreach (var currency in external)
        {
            switch (stored.TryGetValue(currency.Id, out var entity))
            {
                case true when entity.Rate != currency.Rate:
                    entity.Rate = currency.Rate;
                    break;
                case false:
                    dbContext.Currencies.Add(new CurrencyEntity
                    {
                        Id = currency.Id,
                        Name = currency.Name,
                        Rate = currency.Rate
                    });
                    break;
            }
        }

        await dbContext.SaveChangesAsync(token);
    }
}