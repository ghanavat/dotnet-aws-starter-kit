using System.Globalization;
using FluentValidation;
using FluentValidation.Results;
using Ghanavats.DotnetAws.__ENTITIES_NAMESPACE__;
using Ghanavats.DotnetAws.__FEATURE_NAMESPACE__.Contracts;
using Ghanavats.DotnetAws.__FEATURE_NAMESPACE__.GetPersonDetails.Requests;
using Ghanavats.DotnetAws.UseCases.GetPersonDetails;
using Ghanavats.ResultPattern.Enums;
using Microsoft.Extensions.Logging;
using Moq;

namespace Ghanavats.DotnetAws.UseCases.Tests.GetPersonDetailsTests;

public class GetPersonDetailsUseCaseTests
{
    private readonly Mock<IPeopleRepository> _peopleRepositoryMock;
    private readonly Mock<IValidator<GetPersonDetailsRequest>> _validatorMock;
    private readonly Mock<ILogger<GetPersonDetailsUseCase>> _loggerMock;
    private readonly GetPersonDetailsUseCase _sut;
    
    public GetPersonDetailsUseCaseTests()
    {
        _peopleRepositoryMock = new Mock<IPeopleRepository>();
        _validatorMock = new Mock<IValidator<GetPersonDetailsRequest>>();
        _loggerMock = new Mock<ILogger<GetPersonDetailsUseCase>>();
        _sut = new GetPersonDetailsUseCase(_peopleRepositoryMock.Object, _validatorMock.Object, _loggerMock.Object);
    }
    
    [Fact]
    public async Task GetDetails_ShouldReturnInvalidResult_WhenValidationFails()
    {
        //Arrange
        var expectedRequest = new GetPersonDetailsRequest
        {
            PersonId = string.Empty
        };
        
        _validatorMock.Setup(x => x.ValidateAsync(It.IsAny<GetPersonDetailsRequest>())).ReturnsAsync(new ValidationResult
        {
            Errors = [new ValidationFailure("SomeProperty", "SomeProperty is required.")]
        });
        
        _loggerMock.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
        
        //Act
        var actual = await _sut.GetDetails(expectedRequest);
        
        //Assert
        Assert.NotNull(actual);
        Assert.Equal(ResultStatus.Invalid, actual.Status);
    }
    
    [Fact]
    public async Task GetDetails_ShouldReturnSuccessResult_WhenValidationPassed()
    {
        //Arrange
        var expectedRequest = new GetPersonDetailsRequest
        {
            PersonId = Guid.NewGuid().ToString()
        };

        _peopleRepositoryMock.Setup(x => x.GetPersonById(It.IsAny<Guid>()))
            .ReturnsAsync(Person.Create("TestName", "test@domain.com", "123456789", new DateTime(1990, 01, 01).ToString(CultureInfo.InvariantCulture)));
        _validatorMock.Setup(x => x.ValidateAsync(It.IsAny<GetPersonDetailsRequest>())).ReturnsAsync(new ValidationResult());
        _loggerMock.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
        
        //Act
        var actual = await _sut.GetDetails(expectedRequest);
        
        //Assert
        Assert.NotNull(actual);
        Assert.Equal(ResultStatus.Ok, actual.Status);
    }
    
    [Fact]
    public async Task GetDetails_ShouldReturnSuccessResult_WhenRequestedPersonNotFound()
    {
        //Arrange
        var expectedRequest = new GetPersonDetailsRequest
        {
            PersonId = Guid.NewGuid().ToString()
        };

        _peopleRepositoryMock.Setup(x => x.GetPersonById(It.IsAny<Guid>()))
            .ReturnsAsync(new Person());
        _validatorMock.Setup(x => x.ValidateAsync(It.IsAny<GetPersonDetailsRequest>())).ReturnsAsync(new ValidationResult());
        _loggerMock.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
        
        //Act
        var actual = await _sut.GetDetails(expectedRequest);
        
        //Assert
        Assert.NotNull(actual);
        Assert.Equal(ResultStatus.NotFound, actual.Status);
    }
}
