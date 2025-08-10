using ApiGateway.WebApi;
using Microsoft.Extensions.Options;
using Users.Grpc.User;
using static Users.Grpc.User.UserService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<GrpcServiceOptions>("UserGrpc",
    builder.Configuration.GetSection("UserGrpc")
);

builder.Services.Configure<GrpcServiceOptions>("FinanceGrpc",
    builder.Configuration.GetSection("FinanceGrpc")
);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddGrpcClient<UserServiceClient>((services, options) =>
{
    options.Address = services.GetRequiredService<IOptionsSnapshot<GrpcServiceOptions>>().Value.Address;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/users",
        async (UserServiceClient usersBase) =>
        {
            await usersBase.RegisterAsync(new RegisterUserRequest { Username = "wow", Password = "wow" });
        })
    .WithName("RegisterUser")
    .WithOpenApi();

app.Run();