using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using ToyRobot.Models;
using ToyRobot.Repositories;

namespace ToyRobot.Test.Repositories
{
    [TestFixture]
    public class RobotRepositoryTest
    {
        private const string POSITION_KEY = "Position";

        private readonly IRobotRepository _robotRepository;
        private readonly IMemoryCache _cache;

        public RobotRepositoryTest()
        {
            var services = new ServiceCollection();
            services.AddMemoryCache();
            services.AddTransient<IRobotRepository, RobotRepository>();
            var serviceProvider = services.BuildServiceProvider();

            _robotRepository = serviceProvider.GetService<IRobotRepository>();
            _cache = serviceProvider.GetService<IMemoryCache>();
        }
        
        [Test]
        public void GetPostionReturnsSuccessfully()
        {
            // Arrange
            var cachedPosition = new Position(1, 2, DirectionEnum.East);
            _cache.Set(POSITION_KEY, cachedPosition);

            // Act
            var position = _robotRepository.GetPosition();

            // Assert
            Assert.AreEqual(cachedPosition, position);
        }

        [Test]
        public void GetPostionReturnsNullWhenNotSet()
        {
            // Arrange
            
            // Act
            var position = _robotRepository.GetPosition();

            // Assert
            Assert.IsNull(position);
        }

        [Test]
        public void UpdatePositionWhenEmptyIsSuccessful ()
        {
            // Arrange
            
            // Act
            var position = new Position(1, 2, DirectionEnum.East);
            _robotRepository.UpdatePosition(position);

            // Assert
            _cache.TryGetValue(POSITION_KEY, out Position cachedPosition);
            Assert.AreEqual(position, cachedPosition);
        }

        [Test]
        public void UpdatePositionWhenSetIsSuccessful()
        {
            // Arrange
            var position = new Position(1, 2, DirectionEnum.East);
            _cache.Set(POSITION_KEY, position);

            // Act
            var newPosition = new Position(0, 4, DirectionEnum.North);
            _robotRepository.UpdatePosition(newPosition);

            // Assert
            _cache.TryGetValue(POSITION_KEY, out Position cachedPosition);
            Assert.AreEqual(newPosition, cachedPosition);
        }
    }
}
