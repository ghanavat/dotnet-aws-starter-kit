using FluentValidation;
using Ghanavats.DotnetAws.UseCases.GerPersonDetails.Requests;

namespace Ghanavats.DotnetAws.UseCases.GerPersonDetails.Validators;

public sealed class GetPersonDetailsRequestValidator : AbstractValidator<GetPersonDetailsRequest>
{
    public GetPersonDetailsRequestValidator()
    {
        RuleFor(x => x.PersonId)
            .Must(personId => Guid.TryParse(personId, out _)).WithMessage("PersonId must be a valid GUID")
            .NotEmpty().WithMessage("PersonId is required");
    }
}
