using System.Xml.Serialization;
using Finance.Db;

namespace Finance.CurrencyUpdater.Application.Currency;

internal class CbrCurrencyProvider(HttpClient client) : ICurrencyProvider
{
    private readonly XmlSerializer _serializer = new(typeof(CbrCurrencies));

    public async Task<IEnumerable<CurrencyEntity>> GetAll(CancellationToken cancellationToken)
    {
        var currencies = await client.GetFromXml<CbrCurrencies>("", _serializer, cancellationToken);
        return currencies.Currencies
            .Select(c => new CurrencyEntity
            {
                Id = c.Id,
                Name = c.Name,
                Rate = decimal.Parse(c.VunitRate)
            });
    }
}