using FluentValidation;
using Ghanavats.DotnetAws.__FEATURE_NAMESPACE__.Contracts;
using Ghanavats.DotnetAws.__FEATURE_NAMESPACE__.GetPersonDetails.Requests;
using Ghanavats.DotnetAws.__FEATURE_NAMESPACE__.GetPersonDetails.Responses;
using Ghanavats.ResultPattern;
using Microsoft.Extensions.Logging;

namespace Ghanavats.DotnetAws.UseCases.GetPersonDetails;

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
        if (person.Id.Equals(Guid.Empty))
        {
            return Result.NotFound();
        }

        return Result<GetPersonByIdResponse>.Success(person.ToResponse());
    }
}
