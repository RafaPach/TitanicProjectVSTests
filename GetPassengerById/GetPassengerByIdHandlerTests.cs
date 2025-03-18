using System;
using System.Dynamic;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;
using DeveloperPathways.Application.Queries.GetPassengers;
using DeveloperPathways.Domain;
using DeveloperPathways.Dtos;
using DeveloperPathways.Interface;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DeveloperPathways.Application.Tests.Queries.GetPassengers
{
    // Make the class public for xUnit discovery
    public class GetPassengerByIdHandlerTests
    {
        private readonly Mock<IPassengerRepository> _repositoryMock;
        private readonly Mock<IValidator<GetPassengerByIdQuery>> _validatorMock; // Keep this mocked
        private readonly Mock<ILogger<GetPassengerByIdHandler>> _loggerMock;
        private readonly GetPassengerByIdHandler _handler;

        public GetPassengerByIdHandlerTests()
        {
            _repositoryMock = new Mock<IPassengerRepository>();
            _validatorMock = new Mock<IValidator<GetPassengerByIdQuery>>(); // Mocked, even if not used
            _loggerMock = new Mock<ILogger<GetPassengerByIdHandler>>();

            _handler = new GetPassengerByIdHandler(
                _repositoryMock.Object,
                _validatorMock.Object, // Pass the mocked validator
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnPassengerDto_WhenPassengerExistsAndValidationPasses()
        {
            // Arrange
            var query = new GetPassengerByIdQuery(1);

            _validatorMock
                .Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            var passenger = new Passenger
            {
                PassengerId = 1,
                Name = "Test",
                Age = 30,
                Sex = "male"
            };

            _repositoryMock
                .Setup(r => r.GetPassengerByIdAsync(query.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(passenger);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(passenger.PassengerId);
            result.Name.Should().Be(passenger.Name);

            _repositoryMock.Verify(r => r.GetPassengerByIdAsync(query.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowKeyNotFoundException_WhenPassengerDoesNotExist()
        {
            // Arrange
            var query = new GetPassengerByIdQuery(999);

            _validatorMock
                .Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _repositoryMock
                .Setup(r => r.GetPassengerByIdAsync(query.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Passenger?)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

//            Func<Task>:

//This defines a delegate type that takes no parameters(in this case) and returns a Task.
//Task is used because the method you're testing (Handle) is asynchronous and returns a Task.
//async () => await _handler.Handle(query, CancellationToken.None):

//This part defines an asynchronous lambda. It's a shorthand for defining a method in-line without explicitly declaring a method.
//await _handler.Handle(query, CancellationToken.None) invokes the Handle method of your handler and awaits the result.It simulates calling the method asynchronously in your test.
//The Purpose of act:

//You assign this delegate to act. Later in your test, you'll invoke act() to actually run the Handle method as part of your assertion process.
//In unit tests, especially when testing exceptions, you often use this pattern to check if calling the method results in the expected exception.

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage($"Passenger with ID {query.Id} was not found.");

            _repositoryMock.Verify(r => r.GetPassengerByIdAsync(query.Id, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
