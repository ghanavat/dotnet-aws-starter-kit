using System.Diagnostics.CodeAnalysis;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Ghanavats.CleanArchitecture.Infrastructure.Repositories;
using Ghanavats.CleanArchitecture.UseCases.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ghanavats.CleanArchitecture.Infrastructure.DependencyInjection;

[ExcludeFromCodeCoverage]
public static class InfrastructureExtension
{
    extension(IServiceCollection services)
    {
        public void AddRepositories(IConfiguration configuration)
        {
            services.AddScoped<IPeopleRepository, PeopleRepository>();

            services.AddSingleton<DynamoDBContext>(sp =>
            {   
                var clientConfig = new AmazonDynamoDBConfig
                {
                    RegionEndpoint = AwsRegionResolver.Resolve(configuration["AWS:Region"] 
                                                               ?? throw new ArgumentNullException(nameof(configuration)))
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
