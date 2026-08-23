using System.Diagnostics.CodeAnalysis;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Ghanavats.DotnetAws.__FEATURE_NAMESPACE__.Contracts;
using Ghanavats.DotnetAws.__INFRA_TO_FEATURE_NAMESPACE__.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ghanavats.DotnetAws.Infrastructure.DependencyInjection;

[ExcludeFromCodeCoverage]
public static class InfrastructureExtension
{
    extension(IServiceCollection services)
    {
        public void AddRepositories(IConfiguration configuration)
        {
            services.AddScoped<IPeopleRepository, PeopleRepository>();

            services.AddSingleton<IDynamoDBContext>(sp =>
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
