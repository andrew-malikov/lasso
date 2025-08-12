using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Users.Application.Users;
using Users.Grpc.User;
using static Users.Grpc.User.UserService;
using LoginRequest = Users.Grpc.User.LoginRequest;
using LoginResponse = Users.Grpc.User.LoginResponse;

namespace Users.WebApi.Services;

public class UserGrpcService(IUserService service) : UserServiceBase
{
    [AllowAnonymous]
    public override async Task<RegisterUserResponse> Register(RegisterUserRequest request, ServerCallContext context)
    {
        var response =
            await service.Register(new UserDraft { Username = request.Username, Password = request.Password },
                context.CancellationToken);
        return response switch
        {
            SuccessfullyRegistered => new RegisterUserResponse { SuccessfullyRegistered = new Empty() },
            DuplicateUser { Message: var message } => new RegisterUserResponse { FailureMessage = message },
            InvalidUserDraft { Exception: var exception } => new RegisterUserResponse
                { FailureMessage = exception.Message },
            _ => throw new NotImplementedException()
        };
    }

    [AllowAnonymous]
    public override async Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
    {
        var response = await service.Login(new Application.Users.LoginRequest
            { Username = request.Username, Password = request.Password }, context.CancellationToken);
        return response switch
        {
            SuccessfulLogin { Tokens: var tokens } => new LoginResponse
                { Tokens = new TokenPair { AccessToken = tokens.AccessToken, RefreshToken = tokens.RefreshToken } },
            FailedLogin { Message: var message } => new LoginResponse
                { Failure = new LoginFailure { Message = message } },
            _ => throw new NotImplementedException()
        };
    }

    [AllowAnonymous]
    public override async Task<Empty> Logout(LogoutRequest request, ServerCallContext context)
    {
        await service.Logout(request.RefreshToken, context.CancellationToken);
        return new Empty();
    }
}