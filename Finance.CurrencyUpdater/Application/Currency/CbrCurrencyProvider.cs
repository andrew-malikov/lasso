using System.Text;
using System.Xml.Serialization;
using Finance.Db;

namespace Finance.CurrencyUpdater.Application.Currency;

internal class CbrCurrencyProvider(HttpClient client) : ICurrencyProvider
{
    private readonly XmlSerializer _serializer = new(typeof(CbrCurrencies));
    private static readonly Uri DailyCurrenciesUri = new("https://www.cbr.ru/scripts/XML_daily.asp");

    public async Task<IEnumerable<CurrencyEntity>> GetAll(CancellationToken cancellationToken)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        var currencies = await client.GetFromXml<CbrCurrencies>(DailyCurrenciesUri, _serializer, cancellationToken);
        return currencies.Currencies
            .Select(c => new CurrencyEntity
            {
                Id = c.Id,
                Name = c.Name,
                Rate = decimal.Parse(c.VunitRate)
            });
    }
}