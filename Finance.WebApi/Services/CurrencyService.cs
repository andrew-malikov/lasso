using Finance.Grpc.Currency;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using static Finance.Grpc.Currency.CurrencyService;

namespace Finance.WebApi.Services;

public class CurrencyService(ILogger<CurrencyService> logger) : CurrencyServiceBase
{
    private readonly ILogger<CurrencyService> _logger = logger;

    public override Task<CurrenciesResponse> GetFavorites(Empty request, ServerCallContext context)
    {
        return Task.FromResult(new CurrenciesResponse { Currencies = { } });
    }
}