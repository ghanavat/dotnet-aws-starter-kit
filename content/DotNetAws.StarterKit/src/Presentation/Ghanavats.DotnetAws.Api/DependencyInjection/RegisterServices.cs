using System.Diagnostics.CodeAnalysis;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using FluentValidation;
using Ghanavats.DotnetAws.__FEATURE_NAMESPACE__.Contracts;
using Ghanavats.DotnetAws.__FEATURE_NAMESPACE__.GetPersonDetails.Validators;
using Ghanavats.DotnetAws.__INFRA_TO_FEATURE_NAMESPACE__.Repositories;
using Ghanavats.DotnetAws.UseCases.GetPersonDetails;

namespace Ghanavats.DotnetAws.Api.DependencyInjection;

[ExcludeFromCodeCoverage]
internal static class RegisterServices
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

        public void AddRepositories(IConfiguration configuration)
        {
            services.AddScoped<IPeopleRepository, PeopleRepository>();

            services.AddSingleton<DynamoDBContext>(sp =>
            {
                var awsRegion = configuration["AWS:Region"];
                ArgumentException.ThrowIfNullOrWhiteSpace(awsRegion);
                
                var clientConfig = new AmazonDynamoDBConfig
                {
                    RegionEndpoint = AwsRegionResolver.Resolve(awsRegion)
                };
                var client = new AmazonDynamoDBClient(clientConfig);

                return new DynamoDBContextBuilder()
                    .WithDynamoDBClient(() => client)
                    .Build();
            });
        }
    }
}

internal static class AwsRegionResolver
{
    internal static RegionEndpoint Resolve(string region)
    {
        return RegionEndpoint.GetBySystemName(region);
    }
}
