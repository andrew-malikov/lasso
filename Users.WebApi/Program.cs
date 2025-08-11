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
app.Run();