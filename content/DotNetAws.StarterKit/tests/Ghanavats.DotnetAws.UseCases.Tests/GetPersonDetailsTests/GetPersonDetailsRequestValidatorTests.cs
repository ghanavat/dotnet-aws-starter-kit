using FluentValidation.TestHelper;
using Ghanavats.DotnetAws.__FEATURE_NAMESPACE__.GetPersonDetails.Requests;
using Ghanavats.DotnetAws.__FEATURE_NAMESPACE__.GetPersonDetails.Validators;

namespace Ghanavats.DotnetAws.UseCases.Tests.GetPersonDetailsTests;

public class GetPersonDetailsRequestValidatorTests
{
    private readonly GetPersonDetailsRequestValidator _validator;

    public GetPersonDetailsRequestValidatorTests()
    {
        _validator = new GetPersonDetailsRequestValidator();
    }

    [Theory]
    [InlineData("  ")]
    [InlineData("")]
    public async Task ValidatePersonId_ShouldFail_WhenPersonIdIsEmptyOrWhitespaceInvalid(string expectedPersonId)
    {
        //Arrange
        var expectedRequest = new GetPersonDetailsRequest
        {
            PersonId = expectedPersonId
        };

        //Act
        var actual = await _validator.TestValidateAsync(expectedRequest, cancellationToken: TestContext.Current.CancellationToken);
        
        //Assert
        actual.ShouldHaveValidationErrorFor(x => x.PersonId).WithErrorMessage("PersonId is required");
    }
    
    [Fact]
    public async Task ValidatePersonId_ShouldFail_WhenPersonIdIsInvalid()
    {
        //Arrange
        var expectedRequest = new GetPersonDetailsRequest
        {
            PersonId = "Invalid_Guid"
        };

        //Act
        var actual = await _validator.TestValidateAsync(expectedRequest, cancellationToken: TestContext.Current.CancellationToken);
        
        //Assert
        actual.ShouldHaveValidationErrorFor(x => x.PersonId).WithErrorMessage("Invalid PersonId");
    }
    
    [Fact]
    public async Task ValidatePersonId_ShouldPass_WhenPersonIdIsValid()
    {
        //Arrange
        var expectedRequest = new GetPersonDetailsRequest
        {
            PersonId = Guid.NewGuid().ToString()
        };

        //Act
        var actual = await _validator.TestValidateAsync(expectedRequest, cancellationToken: TestContext.Current.CancellationToken);
        
        //Assert
        actual.ShouldNotHaveValidationErrorFor(x => x.PersonId);
    }
}
