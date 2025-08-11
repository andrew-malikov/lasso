using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Users.Grpc.User;
using static Users.Grpc.User.UserService;

namespace Users.WebApi.Services;

public class UserGrpcService() : UserServiceBase
{
    [AllowAnonymous]
    public override Task<Empty> Register(RegisterUserRequest request, ServerCallContext context)
    {
        return Task.FromResult(new Empty());
    }

    [Authorize]
    public override Task<UserTokensResponse> Login(LoginRequest request, ServerCallContext context)
    {
        return base.Login(request, context);
    }

    [AllowAnonymous]
    public override Task<Empty> Logout(LogoutRequest request, ServerCallContext context)
    {
        return base.Logout(request, context);
    }
}