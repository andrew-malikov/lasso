using System.Globalization;
using Finance.Db;
using Finance.Grpc.Currency;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using static Finance.Grpc.Currency.CurrencyService;

namespace Finance.WebApi.Services;

public class CurrencyService(FinanceDbContext dbContext) : CurrencyServiceBase
{
    [Authorize]
    public override async Task<CurrenciesResponse> GetFavorites(Empty request, ServerCallContext context)
    {
        var currencies = await dbContext.Currencies
            .AsNoTracking()
            .Select(c => new CurrencyResponse()
            {
                Id = c.Id,
                Name = c.Name,
                Rate = c.Rate.ToString(CultureInfo.InvariantCulture)
            })
            .ToListAsync(context.CancellationToken);
        var response = new CurrenciesResponse();
        response.Currencies.AddRange(currencies);
        return response;
    }
}