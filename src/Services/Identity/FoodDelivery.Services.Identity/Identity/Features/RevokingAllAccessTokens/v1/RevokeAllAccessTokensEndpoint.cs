using AutoMapper;
using BuildingBlocks.Abstractions.Commands;
using BuildingBlocks.Abstractions.Web.MinimalApi;
using BuildingBlocks.Web.Extensions;
using BuildingBlocks.Web.Minimal.Extensions;
using Humanizer;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FoodDelivery.Services.Identity.Identity.Features.RevokingAllAccessTokens.V1;

public static class RevokeAllAccessTokensEndpoint
{
    public static IEndpointRouteBuilder MapRevokeAllAccessTokensEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints
            .MapPost("/revoke-all-tokens", Handle)
            .RequireAuthorization(IdentityConstants.Role.User)
            .MapToApiVersion(1.0)
            // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/responses?#typedresults-vs-results
            // .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            // .Produces(StatusCodes.Status204NoContent)
            .WithName(nameof(RevokeAllAccessTokens))
            .WithDisplayName(nameof(RevokeAllAccessTokens).Humanize())
            .WithSummaryAndDescription(
                nameof(RevokeAllAccessTokens).Humanize(),
                nameof(RevokeAllAccessTokens).Humanize()
            );

        return endpoints;

        async Task<Results<NoContent, ValidationProblem>> Handle(
            [AsParameters] RevokeAllTokensRequestParameters requestParameters
        )
        {
            var (context, commandProcessor, mapper, cancellationToken) = requestParameters;

            var command = RevokeAllAccessTokens.Of(context.User.Identity!.Name!);

            await commandProcessor.SendAsync(command, cancellationToken);

            // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/responses
            // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/openapi?view=aspnetcore-7.0#multiple-response-types
            return TypedResults.NoContent();
        }
    }
}

internal record RevokeAllTokensRequestParameters(
    HttpContext HttpContext,
    ICommandBus CommandProcessor,
    IMapper Mapper,
    CancellationToken CancellationToken
) : IHttpCommand;
