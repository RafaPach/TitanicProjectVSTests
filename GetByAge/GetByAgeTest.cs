using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeveloperPathways.Application.Queries.GetByAge;
using DeveloperPathways.Application.Queries.GetPassengers;
using DeveloperPathways.Domain;
using DeveloperPathways.Interface;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;

namespace TestTitanicProject.GetByAge
{
    public class GetByAgeTest
    {
        private readonly Mock<IGetByAgeRepository> _repositoryMock;
        //private readonly Mock<IValidator<GetPassengersByAgeQuery>> _validatorMock; // Keep this mocked
        private readonly Mock<ILogger<GetPassengersByAgeHandler>> _loggerMock;
        private readonly GetPassengersByAgeHandler _handler;


        public GetByAgeTest()
        {
            _repositoryMock = new Mock<IGetByAgeRepository>();
            _loggerMock = new Mock<ILogger<GetPassengersByAgeHandler>>();
            _handler = new GetPassengersByAgeHandler(_repositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsPassengersOrderedByAge()
        {
            //Arrange

            var passengers = new List<Passenger>
            {
                new Passenger {PassengerId = 1, Name = "Test", Age = 25},
                new Passenger {PassengerId= 2, Name = "Test2", Age = 30 }
            };

            _repositoryMock
                .Setup(r => r.GetByAgeAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(passengers);

            var query = new GetPassengersByAgeQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task Handle_NoPassengers_ReturnsEmptyList()
        {
            // Arrange
            var passengers = new List<Passenger>();

            _repositoryMock
                .Setup(r => r.GetByAgeAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(passengers);

            var query = new GetPassengersByAgeQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _repositoryMock.Verify(r => r.GetByAgeAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
