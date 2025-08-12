using System.Xml.Serialization;

namespace Finance.CurrencyUpdater.Application;

public static class HttpClientExtensions
{
    public static async Task<T> GetFromXml<T>(this HttpClient client, Uri requestUri, XmlSerializer serializer,
        CancellationToken token)
    {
        var response = await client.GetAsync(requestUri, token);
        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync(token);
        return (T)serializer.Deserialize(stream);
    }
}