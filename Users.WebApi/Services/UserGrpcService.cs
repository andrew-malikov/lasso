using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Users.Grpc.User;
using static Users.Grpc.User.UserService;

namespace Users.WebApi.Services;

public class UserGrpcService(ILogger<UserGrpcService> logger) : UserServiceBase
{
    private readonly ILogger<UserGrpcService> _logger = logger;

    public override Task<Empty> Register(RegisterUserRequest request, ServerCallContext context)
    {
        return Task.FromResult(new Empty());
    }
}