using ApiGateway.WebApi;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Options;
using Users.Grpc.User;
using static Users.Grpc.User.UserService;
using static Finance.Grpc.Currency.CurrencyService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(provider =>
{
    var factory = provider.GetRequiredService<ILoggerFactory>();
    return factory.CreateLogger("ApiGateway");
});

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
builder.Services.AddGrpcClient<CurrencyServiceClient>((services, options) =>
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

app.MapPost("/api/users/register", async (RegisterUserRequest request, UserServiceClient grpcClient) =>
    {
        var grpcResponse = await grpcClient.RegisterAsync(request);
        return grpcResponse.ResultCase switch
        {
            RegisterUserResponse.ResultOneofCase.SuccessfullyRegistered => Results.Ok(),

            RegisterUserResponse.ResultOneofCase.FailureMessage => Results.BadRequest(new
                { error = grpcResponse.FailureMessage }),

            _ => Results.StatusCode(500)
        };
    })
    .WithName("RegisterUser")
    .WithOpenApi();

app.MapPost("/api/users/login", async (LoginRequest request, UserServiceClient grpcClient) =>
    {
        var grpcResponse = await grpcClient.LoginAsync(request);
        return grpcResponse.ResultCase switch
        {
            LoginResponse.ResultOneofCase.Tokens
                => Results.Ok(new
                {
                    access_token = grpcResponse.Tokens.AccessToken,
                    refresh_token = grpcResponse.Tokens.RefreshToken
                }),

            LoginResponse.ResultOneofCase.Failure
                => Results.BadRequest(new { error = grpcResponse.Failure.Message }),

            _ => Results.StatusCode(500)
        };
    })
    .WithName("UserLogin")
    .WithOpenApi();

app.MapPost("/api/users/logout", async (LogoutRequest request, UserServiceClient grpcClient) =>
    {
        await grpcClient.LogoutAsync(request);
        return Results.Ok();
    })
    .WithName("UserLogout")
    .WithOpenApi();


app.MapGet("/api/currencies/favorites",
        async (HttpContext httpContext, CurrencyServiceClient grpcClient, ILogger logger) =>
        {
            var authHeader = httpContext.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return Results.Unauthorized();
            }

            var metadata = new Metadata
            {
                { "Authorization", authHeader }
            };

            try
            {
                var grpcResponse = await grpcClient.GetFavoritesAsync(new Empty(), new CallOptions(headers: metadata));

                var result = grpcResponse.Currencies.Select(c => new CurrencyResponse
                {
                    Id = c.Id,
                    Name = c.Name,
                    Rate = c.Rate
                });

                return Results.Ok(result);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Unauthenticated)
            {
                return Results.Unauthorized();
            }
            catch (RpcException ex)
            {
                logger.LogError(ex, "An error occured during handling GRPC request/response.");
                return Results.StatusCode(500);
            }
        })
    .WithName("UserFavouriteCurrencies")
    .WithOpenApi();

app.Run();

internal class CurrencyResponse
{
    public required string Id { get; init; }

    public required string Name { get; init; }

    public required string Rate { get; init; }
}