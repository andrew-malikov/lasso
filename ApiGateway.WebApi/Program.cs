using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Users.Grpc.User;
using static Users.Grpc.User.UserService;
using static Finance.Grpc.Currency.CurrencyService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(provider =>
{
    var factory = provider.GetRequiredService<ILoggerFactory>();
    return factory.CreateLogger("ApiGateway");
});

builder.Services
    .AddAuthentication("NoSignJwt")
    .AddScheme<AuthenticationSchemeOptions, NoSignatureJwtHandler>("NoSignJwt", null);

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token."
    });
    c.OperationFilter<AuthorizeCheckOperationFilter>();
});

builder.Services.AddGrpcClient<UserServiceClient>((services, options) =>
{
    options.Address = new Uri(builder.Configuration["UserGrpc:Address"]);
});
builder.Services.AddGrpcClient<CurrencyServiceClient>((services, options) =>
{
    options.Address = new Uri(builder.Configuration["FinanceGrpc:Address"]);
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

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
        [Authorize] async (HttpContext httpContext, CurrencyServiceClient grpcClient, ILogger logger) =>
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

app.UseAuthentication();
app.UseAuthorization();

app.Run();

internal class CurrencyResponse
{
    public required string Id { get; init; }

    public required string Name { get; init; }

    public required string Rate { get; init; }
}

public class AuthorizeCheckOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var hasAuthorize =
            context.MethodInfo.DeclaringType?.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() == true
            || context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();

        if (hasAuthorize)
        {
            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                }
            };
        }
    }
}

public class NoSignatureJwtHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    private static readonly JwtSecurityTokenHandler TokenHandler = new();

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("Authorization", out var authHeaders))
            return Task.FromResult(AuthenticateResult.Fail("Missing Authorization Header."));

        var token = authHeaders.ToString().Replace("Bearer ", "");

        var jwt = TokenHandler.ReadJwtToken(token);

        var identity = new ClaimsIdentity(jwt.Claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}