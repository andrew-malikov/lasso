using Microsoft.EntityFrameworkCore;
using Users.Db;
using Users.WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextPool<UsersDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("UsersDbContext"), x => x.MigrationsAssembly("Users.Db"))
        .UseSnakeCaseNamingConvention());

builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<UserGrpcService>();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();