using System.Xml.Serialization;

namespace Finance.CurrencyUpdater.Application.Currency;

public class CbrCurrency
{
    [XmlAttribute("ID")]
    public string Id { get; set; }

    [XmlElement("NumCode")]
    public int NumCode { get; set; }

    [XmlElement("CharCode")]
    public string CharCode { get; set; }

    [XmlElement("Nominal")]
    public int Nominal { get; set; }

    [XmlElement("Name")]
    public string Name { get; set; }

    [XmlElement("Value")]
    public string Value { get; set; }

    [XmlElement("VunitRate")]
    public string VunitRate { get; set; }
}