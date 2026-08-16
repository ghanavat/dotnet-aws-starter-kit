using System.Diagnostics.CodeAnalysis;
using Ghanavats.DotnetAws.Api.Endpoints;
using Ghanavats.DotnetAws.Api.Extensions;

namespace Ghanavats.DotnetAws.Api.DependencyInjection;

[ExcludeFromCodeCoverage]
public static class RegisterMinimalEndpoints
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
