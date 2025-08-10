using System.Diagnostics.CodeAnalysis;

namespace ApiGateway.WebApi;

internal class GrpcServiceOptions
{
    [NotNull] public Uri? Address { get; set; } = null;
}