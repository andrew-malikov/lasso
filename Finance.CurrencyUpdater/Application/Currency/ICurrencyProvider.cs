using Finance.Db;

namespace Finance.CurrencyUpdater.Application.Currency;

public interface ICurrencyProvider
{
    public Task<IEnumerable<CurrencyEntity>> GetAll(CancellationToken cancellationToken);
}