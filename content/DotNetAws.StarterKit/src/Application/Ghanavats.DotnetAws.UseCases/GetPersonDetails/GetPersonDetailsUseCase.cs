using FluentValidation;
using Ghanavats.DotnetAws.__FEATURE_NAMESPACE__.GetPersonDetails.Requests;
using Ghanavats.DotnetAws.__FEATURE_NAMESPACE__.GetPersonDetails.Responses;
using Ghanavats.DotnetAws.__USECASES_CONTRACT_TO_API_FEATURE_NAMESPACE__;
using Ghanavats.ResultPattern;
using Microsoft.Extensions.Logging;

namespace Ghanavats.DotnetAws.__FEATURE_NAMESPACE__.GetPersonDetails;

public interface IGetPersonDetails
{
    Task<Result<GetPersonByIdResponse>> GetDetails(GetPersonDetailsRequest request);
}

/// <summary>
/// A greatly simplified sample use-case to fetch/create Person data.
/// </summary>
public sealed class GetPersonDetailsUseCase : IGetPersonDetails
{
    private readonly IPeopleRepository _peopleRepository;
    private readonly IValidator<GetPersonDetailsRequest> _validator;
    private readonly ILogger<GetPersonDetailsUseCase> _logger;

    public GetPersonDetailsUseCase(IPeopleRepository peopleRepository,
        IValidator<GetPersonDetailsRequest> validator,
        ILogger<GetPersonDetailsUseCase> logger)
    {
        _peopleRepository = peopleRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<GetPersonByIdResponse>> GetDetails(GetPersonDetailsRequest request)
    {
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            if (_logger.IsEnabled(LogLevel.Error))
            {
                _logger.LogError("{ValidationResult}", validationResult.Errors);
            }

            return Result.Invalid(validationResult);
        }

        var person = await _peopleRepository.GetPersonById(Guid.Parse(request.PersonId));
        
        if (person is null)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Person {PersonId} not found.", request.PersonId);
            }
            return Result.NotFound();
        }
        
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Successfully fetched Person details for PersonId: {PersonId}", request.PersonId);
        }

        return Result<GetPersonByIdResponse>.Success(person.ToResponse());
    }
}
