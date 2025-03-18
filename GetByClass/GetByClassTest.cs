using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeveloperPathways.Application.Queries.GetByClass;
using DeveloperPathways.Domain;
using DeveloperPathways.Dtos;
using DeveloperPathways.Interface;
using Moq;

namespace TestTitanicProject.GetByClass
{
    public class GetByClassTest
    {
        private readonly Mock<IGetByClassRepository> _mockGetByClassRepository;
        private readonly GetByClassHandler _handler;

        public GetByClassTest()
        {
            _mockGetByClassRepository = new Mock<IGetByClassRepository>();
            _handler = new GetByClassHandler(_mockGetByClassRepository.Object);
        }

        [Fact]
        public async Task Handle_ReturnsClassBreakdown()
        {
            // Arrange

            var query = new GetByClassQuery();

            var passengers = new List<Passenger>
            {
                new Passenger { PassengerId = 1, Name = "Test", Pclass = 1, Survived = true },
                new Passenger { PassengerId = 2, Name = "Test2", Pclass = 2, Survived = false },
                new Passenger { PassengerId = 3, Name = "Test3", Pclass = 3, Survived = true }
            };

            var expectedClassBreakdown = new FinalClassBreakDown
            {
                ClassBreakdown =new ClassAggregdationDto
                {
                    FirstClass = new List<PassengerDto>
                    {
                        new PassengerDto {Id = 1, Name = "Test"}
                    },
                    SecondClass = new List<PassengerDto>
                    {
                        new PassengerDto {Id = 2 , Name = "Test2"}

                    },
                    ThirdClass = new List<PassengerDto>
                    {
                        new PassengerDto { Id = 3, Name = "Test3" }
                    }
                }
            };

            _mockGetByClassRepository
             .Setup(r => r.GetAllPassengersAsync(It.IsAny<CancellationToken>()))
             .ReturnsAsync(passengers);


            // Act

            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.ClassBreakdown);

            Assert.Equal(expectedClassBreakdown.ClassBreakdown.FirstClass.Count, result.ClassBreakdown.FirstClass.Count);
            Assert.Contains(result.ClassBreakdown.FirstClass, p => p.Name == "Test");
        }

        [Fact]
        public async Task Handle_NoPassengers_ReturnsEmptyClassBreakdown()
        {
            // Arrange
            var passengers = new List<Passenger>();

            var expectedClassBreakdown = new FinalClassBreakDown
            {
                ClassBreakdown = new ClassAggregdationDto
                {
                    FirstClass = new List<PassengerDto>(),
                    SecondClass = new List<PassengerDto>(),
                    ThirdClass = new List<PassengerDto>()
                }
            };

            _mockGetByClassRepository
                .Setup(r => r.GetAllPassengersAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(passengers);

            var query = new GetByClassQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.ClassBreakdown);
            Assert.Empty(result.ClassBreakdown.FirstClass);
            _mockGetByClassRepository.Verify(r => r.GetAllPassengersAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
