using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Ghanavats.DotnetAws.__FEATURE_NAMESPACE__.GetPersonDetails.Validators;
using Ghanavats.DotnetAws.UseCases.GetPersonDetails;
using Microsoft.Extensions.DependencyInjection;

namespace Ghanavats.DotnetAws.UseCases.DependencyInjection;

[ExcludeFromCodeCoverage]
public static class RegisterApplicationServices
{
    extension(IServiceCollection services)
    {
        public void AddValidators()
        {
            services.AddValidatorsFromAssemblyContaining<GetPersonDetailsRequestValidator>();
        }

        public void AddUseCases()
        {
            services.AddScoped<IGetPersonDetails, GetPersonDetailsUseCase>();
        }
    }
}
