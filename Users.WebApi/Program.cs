using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Users.Db;
using Users.WebApi.Services;

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


builder.Services.AddDbContextPool<UsersDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("UsersDbContext"), x => x.MigrationsAssembly("Users.Db"))
        .UseSnakeCaseNamingConvention());

builder.Services.AddAuthorization();
builder.Services.AddGrpc();
var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.MapGrpcService<UserGrpcService>();
app.Run();