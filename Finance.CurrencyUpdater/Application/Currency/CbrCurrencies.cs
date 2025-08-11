using System.Xml.Serialization;

namespace Finance.CurrencyUpdater.Application.Currency;

[XmlRoot("ValCurs")]
public class CbrCurrencies
{
    [XmlAttribute("Date")]
    public string Date { get; set; }

    [XmlAttribute("name")]
    public string Name { get; set; }

    [XmlElement("Valute")]
    public List<CbrCurrency> Currencies { get; set; }
}