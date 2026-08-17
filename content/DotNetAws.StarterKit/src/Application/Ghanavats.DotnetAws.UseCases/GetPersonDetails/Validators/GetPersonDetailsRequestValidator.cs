using FluentValidation;
using Ghanavats.DotnetAws.__FEATURE_NAMESPACE__.GetPersonDetails.Requests;

namespace Ghanavats.DotnetAws.__FEATURE_NAMESPACE__.GetPersonDetails.Validators;

public sealed class GetPersonDetailsRequestValidator : AbstractValidator<GetPersonDetailsRequest>
{
    public GetPersonDetailsRequestValidator()
    {
        RuleFor(x => x.PersonId)
            .NotEmpty().WithMessage("PersonId is required")
            .Must(personId => Guid.TryParse(personId, out _)).WithMessage("PersonId must be a valid GUID");
    }
}
