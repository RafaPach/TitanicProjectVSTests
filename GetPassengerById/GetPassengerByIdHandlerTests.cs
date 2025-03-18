using System;
using System.Threading;
using System.Threading.Tasks;
using DeveloperPathways.Application.Queries.GetPassengers;
using DeveloperPathways.Domain;
using DeveloperPathways.Interface;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DeveloperPathways.Application.Tests.Queries.GetPassengers
{
    public class GetPassengerByIdHandlerTests
    {
        private readonly Mock<IPassengerRepository> _repositoryMock;
        private readonly Mock<IValidator<GetPassengerByIdQuery>> _validatorMock;
        private readonly Mock<ILogger<GetPassengerByIdHandler>> _loggerMock;
        private readonly GetPassengerByIdHandler _handler;

        public GetPassengerByIdHandlerTests()
        {
            _repositoryMock = new Mock<IPassengerRepository>();
            _validatorMock = new Mock<IValidator<GetPassengerByIdQuery>>();
            _loggerMock = new Mock<ILogger<GetPassengerByIdHandler>>();

            _handler = new GetPassengerByIdHandler(
                _repositoryMock.Object,
                _validatorMock.Object,
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
                .ReturnsAsync(new ValidationResult()); // No validation errors

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
                .ReturnsAsync(new ValidationResult()); // No validation errors

            _repositoryMock
                .Setup(r => r.GetPassengerByIdAsync(query.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Passenger?)null); // Passenger not found

            // Act & Assert — Let exception propagate naturally
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(query, CancellationToken.None));

            _repositoryMock.Verify(r => r.GetPassengerByIdAsync(query.Id, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
