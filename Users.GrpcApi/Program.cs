using Microsoft.EntityFrameworkCore;
using Users.Db;
using Users.GrpcApi.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextPool<UsersDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("UsersDbContext"))
        .UseSnakeCaseNamingConvention());

builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<UsersGrpcService>();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();