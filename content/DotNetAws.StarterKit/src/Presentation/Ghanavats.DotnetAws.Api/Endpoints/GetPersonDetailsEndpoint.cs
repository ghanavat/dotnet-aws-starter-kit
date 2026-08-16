using System.Diagnostics.CodeAnalysis;
using Ghanavats.DotnetAws.Api.Extensions;
using Ghanavats.DotnetAws.UseCases.GerPersonDetails;
using Ghanavats.DotnetAws.UseCases.GerPersonDetails.Requests;
using Ghanavats.DotnetAws.UseCases.GerPersonDetails.Responses;
using Ghanavats.ResultPattern;
using Ghanavats.ResultPattern.Mapping;
using Microsoft.AspNetCore.Mvc;

namespace Ghanavats.DotnetAws.Api.Endpoints;

[ExcludeFromCodeCoverage]
public static class GetPersonDetailsEndpoint
{
    public static void Get(WebApplication app)
    {
        app.PeopleGroup().MapGet("/{personId}",
                async ([FromRoute] string personId, IGetPersonDetails getPersonDetails) =>
                {
                    var request = new GetPersonDetailsRequest
                    {
                        PersonId = personId
                    };

                    var result = await getPersonDetails.GetDetails(request);
                    
                    return await result.ToResultAsync();
                })
            .Produces<Result<GetPersonByIdResponse>>()
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
            .WithName("GetPersonDetails")
            .WithTags("PeopleGroup")
            .WithDescription("A simple get endpoint to fetch person details associated with the specified id.");
    }
}
