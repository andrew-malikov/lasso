using System.Globalization;
using System.Security.Claims;
using Finance.Db;
using Finance.Grpc.Currency;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using static Finance.Grpc.Currency.CurrencyService;

namespace Finance.WebApi.Services;

public class CurrencyService(FinanceDbContext dbContext, ILogger<CurrencyService> logger) : CurrencyServiceBase
{
    [Authorize]
    public override async Task<CurrenciesResponse> GetFavorites(Empty request, ServerCallContext context)
    {
        var user = context.GetHttpContext().User;
        logger.LogInformation("User ${@user} requested to get favorite currencies.",
            user.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var currencies = await dbContext.Currencies
            .AsNoTracking()
            .Select(c => new CurrencyResponse
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