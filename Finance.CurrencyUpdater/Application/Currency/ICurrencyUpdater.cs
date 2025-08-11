namespace Finance.CurrencyUpdater.Application.Currency;

public interface ICurrencyUpdater
{
    Task Update(CancellationToken token);
}