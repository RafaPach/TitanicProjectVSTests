using System.Threading;
using System.Threading.Tasks;
using DeveloperPathways.Application.Queries.GetSurvival;
using DeveloperPathways.Domain;
using DeveloperPathways.Interface;
using DeveloperPathways.Models;
using Moq;

namespace TestTitanicProject.GetSurvivals
{
    public class GetSurvivalRateTest
    {
        private readonly Mock<IGetSurivalRepository> _repositoryMock;
        private readonly GetSurvivalsHandler _handler;

        public GetSurvivalRateTest()
        {
            _repositoryMock = new Mock<IGetSurivalRepository>();
            _handler = new GetSurvivalsHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsSurvivalRates()
        {
            // Arrange
            var totalMales = 100;
            var totalFemales = 150;

            // Mock repository to return total male and female counts
            _repositoryMock
                .Setup(r => r.GetTotalMalesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(totalMales);

            _repositoryMock
                .Setup(r => r.GetTotalFemalesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(totalFemales);

            // Mock repository to return survival data
            var passengers = new List<Passenger>
            {
                new Passenger { PassengerId = 1, Sex = "male", Survived = true },  // Survived male
                new Passenger { PassengerId = 2, Sex = "male", Survived = false }, // Perished male
                new Passenger { PassengerId = 3, Sex = "female", Survived = true }, // Survived female
                new Passenger { PassengerId = 4, Sex = "female", Survived = false } // Perished female
            };

            _repositoryMock
                .Setup(r => r.GetAllPassengersAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(passengers);

            var query = new GetSurvivalsQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.SurvivalRates);

            // Verify survival rates for males
            Assert.Equal(1.0 / totalMales * 100, result.SurvivalRates.Survived.Male); // 1 survived male out of 100
            Assert.Equal(1.0 / totalMales * 100, result.SurvivalRates.Perished.Male); // 1 perished male out of 100

            // Verify survival rates for females
            Assert.Equal(1.0 / totalFemales * 100, result.SurvivalRates.Survived.Female); // 1 survived female out of 150
            Assert.Equal(1.0 / totalFemales * 100, result.SurvivalRates.Perished.Female); // 1 perished female out of 150

            _repositoryMock.Verify(r => r.GetTotalMalesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _repositoryMock.Verify(r => r.GetTotalFemalesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _repositoryMock.Verify(r => r.GetAllPassengersAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_NoData_ReturnsEmptySurvivalRates()
        {
            // Arrange
            var totalMales = 0;
            var totalFemales = 0;
            var passengers = new List<Passenger>(); // No passengers

            _repositoryMock
                .Setup(r => r.GetTotalMalesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(totalMales);

            _repositoryMock
                .Setup(r => r.GetTotalFemalesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(totalFemales);

            _repositoryMock
                .Setup(r => r.GetAllPassengersAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(passengers);

            var query = new GetSurvivalsQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.SurvivalRates);

            // Verify all survival rates are 0
            Assert.Equal(0, result.SurvivalRates.Survived.Male);
            Assert.Equal(0, result.SurvivalRates.Survived.Female);
            Assert.Equal(0, result.SurvivalRates.Perished.Male);
            Assert.Equal(0, result.SurvivalRates.Perished.Female);

            _repositoryMock.Verify(r => r.GetTotalMalesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _repositoryMock.Verify(r => r.GetTotalFemalesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _repositoryMock.Verify(r => r.GetAllPassengersAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
