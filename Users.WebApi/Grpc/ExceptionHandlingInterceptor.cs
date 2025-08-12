using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Users.WebApi.Grpc;

public class ExceptionHandlingInterceptor(ILogger<ExceptionHandlingInterceptor> logger) : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Unhandled exception occurred during GRPC request processing.");
            throw new RpcException(new Status(StatusCode.Internal, "An unexpected error occurred."));
        }
    }
}