using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Ghanavats.DotnetAws.UseCases.GerPersonDetails;
using Ghanavats.DotnetAws.UseCases.GerPersonDetails.Validators;
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
