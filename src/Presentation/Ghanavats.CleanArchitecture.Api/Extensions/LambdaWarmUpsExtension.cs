namespace Ghanavats.CleanArchitecture.Api.Extensions;

internal static class LambdaWarmUpsExtension
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddLambdaWarmUps()
        {
            services.AddAWSLambdaBeforeSnapshotRequest(
                new HttpRequestMessage(HttpMethod.Get, $"api/people/{Guid.Empty}"));
            
            // Add additional warmups here
            
            return services;
        }
    }
}
