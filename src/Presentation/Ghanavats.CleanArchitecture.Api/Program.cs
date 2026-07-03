using Ghanavats.CleanArchitecture.Api.DependencyInjection;
using Ghanavats.CleanArchitecture.Api.Extensions;
using Ghanavats.CleanArchitecture.Api.HealthChecks;
using Ghanavats.CleanArchitecture.Api.Middleware;
using Ghanavats.CleanArchitecture.Infrastructure.DependencyInjection;
using Ghanavats.CleanArchitecture.UseCases.DependencyInjection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add AWS Lambda support. When application is run in Lambda Kestrel is swapped out as the web server with Amazon.Lambda.AspNetCoreServer. This
// package will act as the webserver translating request and responses between the Lambda event source and ASP.NET Core.
builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi);
builder.Services.AddLambdaWarmUps();

// Add application dependencies
builder.Services.AddValidators();
builder.Services.AddUseCases();

// Add Infrastructure dependencies
builder.Services.AddRepositories(builder.Configuration);

builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi("aws_cleanArchitecture_starterKit");

builder.Services.AddHostedService<StartupBackgroundService>();
builder.Services.AddSingleton<StartupHealthCheck>();
builder.Services.AddHealthChecks().AddCheck<StartupHealthCheck>("startup_health_check" , tags: ["ready"]);

builder.Services.AddExceptionHandler<ExceptionHandlerMiddleware>();
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        // If you want to customise how ProblemDetails is defined and put together.
    };
});

var app = builder.Build();

app.UseExceptionHandler();
app.RegisterEndpoints();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = healthCheck => healthCheck.Tags.Contains("ready")
});
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false
});

await app.RunAsync();
