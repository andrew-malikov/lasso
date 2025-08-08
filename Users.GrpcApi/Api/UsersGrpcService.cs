using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace Users.GrpcApi.Api;

public class UsersGrpcService(ILogger<UsersGrpcService> logger) : Users.UsersBase
{
    private readonly ILogger<UsersGrpcService> _logger = logger;

    public override Task<Empty> Register(RegisterUserRequest request, ServerCallContext context)
    {
        return Task.FromResult(new Empty());
    }
}