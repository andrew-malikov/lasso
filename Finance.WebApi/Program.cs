using System.Security.Cryptography.X509Certificates;
using Finance.Db;
using Finance.WebApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextPool<FinanceDbContext>(o =>
{
    o.UseNpgsql(builder.Configuration.GetConnectionString("FinanceDbContext"), x => x.MigrationsAssembly("Finance.Db"));
    o.UseSnakeCaseNamingConvention();
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var publicCert = new X509Certificate2(builder.Configuration["Authentication:VerifyingCertificate"]!);
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Authentication:Issuer"],
            ValidAudience = builder.Configuration["Authentication:Audience"],
            IssuerSigningKey = new X509SecurityKey(publicCert)
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddGrpc();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGrpcService<CurrencyService>();
app.Run();