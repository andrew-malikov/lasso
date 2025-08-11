using System.Security.Cryptography.X509Certificates;
using Finance.WebApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddGrpc();
var app = builder.Build();
app.MapGrpcService<CurrencyService>();
app.Run();