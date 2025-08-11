using Finance.CurrencyUpdater;
using Finance.CurrencyUpdater.Application.Currency;
using Finance.Db;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextPool<FinanceDbContext>(o =>
{
    o.UseNpgsql(builder.Configuration.GetConnectionString("FinanceDbContext"), x => x.MigrationsAssembly("Finance.Db"));
    o.UseSnakeCaseNamingConvention();
});

builder.Services.AddCurrencyServices();
builder.Services.AddHostedService<TimelyCurrencyUpdater>();

var app = builder.Build();
app.Run();