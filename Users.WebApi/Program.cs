using Users.Application;
using Users.Db;
using Users.WebApi.Grpc;
using Users.WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDistributedMemoryCache();
builder.Services.AddJwtServices(builder.Configuration.GetRequiredSection("Authentication"));
builder.Services.AddUserPersistence(builder.Configuration);
builder.Services.AddUserServices();
builder.Services.AddAuthorization();
builder.Services.AddGrpc(o => { o.Interceptors.Add<ExceptionHandlingInterceptor>(); });

var app = builder.Build();

app.MapGrpcService<UserGrpcService>();
app.Run();