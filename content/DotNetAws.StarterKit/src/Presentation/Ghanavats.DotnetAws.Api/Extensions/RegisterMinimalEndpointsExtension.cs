using System.Diagnostics.CodeAnalysis;
using Ghanavats.DotnetAws.Api.Endpoints;

namespace Ghanavats.DotnetAws.Api.Extensions;

[ExcludeFromCodeCoverage]
public static class RegisterMinimalEndpointsExtension
{
    extension(WebApplication app)
    {
        public void RegisterEndpoints()
        {
            app.PeopleGroup();
            GetPersonDetailsEndpoint.Get(app);
        }
    }
}
