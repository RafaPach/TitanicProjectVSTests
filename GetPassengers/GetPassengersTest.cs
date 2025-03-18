using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeveloperPathways.Application.Queries.GetPassengers;
using DeveloperPathways.Domain;
using DeveloperPathways.Dtos;
using DeveloperPathways.Interface;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;

namespace TestTitanicProject.GetPassengers
{
    public class GetPassengersTest
    {
        private readonly Mock<IPassengerRepository> _repositoryMock;
        private readonly Mock<IValidator<GetAllPassengersQuery>> _validatorMock; // Keep this mocked
        private readonly Mock<ILogger<GetAllPassengersHandler>> _loggerMock;
        private readonly GetAllPassengersHandler _handler;
        

    public GetPassengersTest()
        {
            _repositoryMock = new Mock<IPassengerRepository>();
            _validatorMock = new Mock<IValidator<GetAllPassengersQuery>>(); // Mocked, even if not used
            _loggerMock = new Mock<ILogger<GetAllPassengersHandler>>();

            _handler = new GetAllPassengersHandler(
                _repositoryMock.Object,
                _validatorMock.Object, // Pass the mocked validator
                _loggerMock.Object
            );
        }

        [Fact]

        public async Task Handle_ValidQuery_ReturnsPassengers()
        {
            // Arrange

            var query = new GetAllPassengersQuery();

            var passengers = new List<Passenger>
            {
                new Passenger { PassengerId = 1, Name = "Test", Survived = true },
                new Passenger { PassengerId = 2, Name = "Test2", Survived = false }
            };

            _validatorMock.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _repositoryMock.Setup(r => r.GetPassengersAsync(query.Survived, It.IsAny<CancellationToken>()))
                .ReturnsAsync(passengers);

            // Act 
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert

            Assert.NotNull(result);
            Assert.Equal(2, passengers.Count());
            _validatorMock.Verify(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()), Times.Once);
            _repositoryMock.Verify(r => r.GetPassengersAsync(query.Survived, It.IsAny<CancellationToken>()), Times.Once);

        }
        [Fact]
        public async Task Handle_NoPassengers_ReturnsEmptyList()
        {
            // Arrange

            var query = new GetAllPassengersQuery { Survived = false };

            _validatorMock.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _repositoryMock.Setup(r => r.GetPassengersAsync(query.Survived, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Passenger>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _validatorMock.Verify(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()), Times.Once);
            _repositoryMock.Verify(r => r.GetPassengersAsync(query.Survived, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_InvalidQuery_ThrowsValidationException()
        {
            // Arrange
            var query = new GetAllPassengersQuery { Survived = true };
            var validationErrors = new List<ValidationFailure>
    {
        new ValidationFailure("Survived", "Survived must be true, false, or null.")
    };

            _validatorMock
                .Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(validationErrors));

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(query, CancellationToken.None));
            _validatorMock.Verify(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()), Times.Once);
            _repositoryMock.Verify(r => r.GetPassengersAsync(It.IsAny<bool?>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
    }
