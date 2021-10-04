using Moq;
using NUnit.Framework;
using ToyRobot.Models;
using ToyRobot.Repositories;
using ToyRobot.Services;

namespace ToyRobot.Test.Services
{
    [TestFixture]
    public class RobotServiceTest
    {
        private Mock<IRobotRepository> _robotRepositoryMock;

        [SetUp]
        public void Setup()
        {
            _robotRepositoryMock = new Mock<IRobotRepository>();
        }

        #region Place Command Tests
        [Test]
        [TestCase(0, 0, DirectionEnum.North)]
        [TestCase(5, 3, DirectionEnum.South)]
        [TestCase(4, 5, DirectionEnum.East)]
        public void PlaceCommandIsSuccessfulWhenNotSet(int x, int y, DirectionEnum direction)
        {
            // Arrange
            var newPosition = new Position(x, y, direction);
            var command = new CommandRequest(CommandEnum.Place, newPosition);
            
            _robotRepositoryMock.Setup(x => x.GetPosition()).Returns((Position)null);
            _robotRepositoryMock.Setup(x => x.UpdatePosition(newPosition)).Returns(true);

            var robotService = new RobotService(_robotRepositoryMock.Object);

            // Act
            var result = robotService.ProcessCommand(command);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(newPosition, result.Position);
        }

        [Test]
        public void PlaceCommandWithDirectionIsSuccessfulWhenPreviouslySet()
        {
            // Arrange
            var newPosition = new Position(1, 5, DirectionEnum.South);
            var command = new CommandRequest(CommandEnum.Place, newPosition);
            var currentPosition = new Position(3, 4, DirectionEnum.North);

            _robotRepositoryMock.Setup(x => x.GetPosition()).Returns(currentPosition);
            _robotRepositoryMock.Setup(x => x.UpdatePosition(newPosition)).Returns(true);

            var robotService = new RobotService(_robotRepositoryMock.Object);

            // Act
            var result = robotService.ProcessCommand(command);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(newPosition, result.Position);
        }

        [Test]
        public void PlaceCommandWithoutDirectionIsSuccessfulWhenPreviouslySet()
        {
            // Arrange
            var newPosition = new Position(1, 5);
            var command = new CommandRequest(CommandEnum.Place, newPosition);
            var currentPosition = new Position(3, 4, DirectionEnum.North);

            _robotRepositoryMock.Setup(x => x.GetPosition()).Returns(currentPosition);
            _robotRepositoryMock.Setup(x => x.UpdatePosition(newPosition)).Returns(true);

            var robotService = new RobotService(_robotRepositoryMock.Object);

            // Act
            var result = robotService.ProcessCommand(command);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(newPosition, result.Position);
        }

        [Test]
        public void PlaceCommandWithoutDirectionReturnsErrorWhenNotPreviouslySet()
        {
            // Arrange
            var newPosition = new Position(1, 5);
            var command = new CommandRequest(CommandEnum.Place, newPosition);

            _robotRepositoryMock.Setup(x => x.GetPosition()).Returns((Position)null);

            var robotService = new RobotService(_robotRepositoryMock.Object);

            // Act
            var result = robotService.ProcessCommand(command);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(RobotService.NOT_VALID_ERROR, result.ErrorMessage);
            _robotRepositoryMock.Verify(x => x.UpdatePosition(It.IsAny<Position>()), Times.Never);
        }

        [Test]
        [TestCase(-1, 0, DirectionEnum.North)]
        [TestCase(5, 7, DirectionEnum.South)]
        [TestCase(6, -8, DirectionEnum.East)]
        public void PlaceCommandOutOfBoundsReturnsError(int x, int y, DirectionEnum direction)
        {
            // Arrange
            var newPosition = new Position(x, y, direction);
            var command = new CommandRequest(CommandEnum.Place, newPosition);

            _robotRepositoryMock.Setup(x => x.GetPosition()).Returns((Position)null);

            var robotService = new RobotService(_robotRepositoryMock.Object);

            // Act
            var result = robotService.ProcessCommand(command);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(RobotService.NOT_VALID_ERROR, result.ErrorMessage);
            _robotRepositoryMock.Verify(x => x.UpdatePosition(It.IsAny<Position>()), Times.Never);
        }
        #endregion

        #region Move Command Tests
        [Test]
        [TestCase(0, 0, DirectionEnum.North, 0, 1)]
        [TestCase(5, 3, DirectionEnum.South, 5, 2)]
        [TestCase(4, 5, DirectionEnum.East, 5, 5)]
        [TestCase(3, 3, DirectionEnum.West, 2, 3)]
        public void MoveCommandWithPlacementIsSuccessful(int x, int y, DirectionEnum direction, int newX, int newY)
        {
            // Arrange
            var command = new CommandRequest(CommandEnum.Move);
            var currentPosition = new Position(x, y, direction);
            var newPosition = new Position(newX, newY, direction);

            _robotRepositoryMock.Setup(x => x.GetPosition()).Returns(currentPosition);
            _robotRepositoryMock.Setup(x => x.UpdatePosition(It.IsAny<Position>())).Returns(true);

            var robotService = new RobotService(_robotRepositoryMock.Object);

            // Act
            var result = robotService.ProcessCommand(command);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(newPosition.Direction, result.Position.Direction);
        }

        [Test]
        public void MoveCommandWithoutPlacementReturnsError()
        {
            // Arrange
            var command = new CommandRequest(CommandEnum.Move);

            _robotRepositoryMock.Setup(x => x.GetPosition()).Returns((Position)null);

            var robotService = new RobotService(_robotRepositoryMock.Object);

            // Act
            var result = robotService.ProcessCommand(command);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(RobotService.NOT_PLACED_ERROR, result.ErrorMessage);
            _robotRepositoryMock.Verify(x => x.UpdatePosition(It.IsAny<Position>()), Times.Never);
        }

        [Test]
        [TestCase(0, 5, DirectionEnum.North)]
        [TestCase(5, 0, DirectionEnum.South)]
        [TestCase(5, 2, DirectionEnum.East)]
        [TestCase(0, 1, DirectionEnum.West)]
        public void MoveCommandOutOfBoundsReturnsError(int x, int y, DirectionEnum direction)
        {
            // Arrange
            var command = new CommandRequest(CommandEnum.Move);
            var currentPosition = new Position(x, y, direction);

            _robotRepositoryMock.Setup(x => x.GetPosition()).Returns(currentPosition);

            var robotService = new RobotService(_robotRepositoryMock.Object);

            // Act
            var result = robotService.ProcessCommand(command);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(RobotService.NOT_VALID_ERROR, result.ErrorMessage);
            _robotRepositoryMock.Verify(x => x.UpdatePosition(It.IsAny<Position>()), Times.Never);
        }
        #endregion

        #region Left Command Tests
        [Test]
        [TestCase(DirectionEnum.East, DirectionEnum.North)]
        [TestCase(DirectionEnum.West, DirectionEnum.South)]
        [TestCase(DirectionEnum.North, DirectionEnum.West)]
        [TestCase(DirectionEnum.South, DirectionEnum.East)]
        public void LeftCommandWithPlacementIsSuccessful(DirectionEnum existingDirection, DirectionEnum newDirection)
        {
            // Arrange
            var command = new CommandRequest(CommandEnum.Left);
            var currentPosition = new Position(3, 4, existingDirection);
            var newPosition = new Position(currentPosition.X, currentPosition.Y, newDirection);

            _robotRepositoryMock.Setup(x => x.GetPosition()).Returns(currentPosition);
            _robotRepositoryMock.Setup(x => x.UpdatePosition(It.IsAny<Position>())).Returns(true);

            var robotService = new RobotService(_robotRepositoryMock.Object);

            // Act
            var result = robotService.ProcessCommand(command);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(newDirection, result.Position.Direction);
        }
        #endregion

        #region Right Command Tests
        [Test]
        [TestCase(DirectionEnum.East, DirectionEnum.South)]
        [TestCase(DirectionEnum.West, DirectionEnum.North)]
        [TestCase(DirectionEnum.North, DirectionEnum.East)]
        [TestCase(DirectionEnum.South, DirectionEnum.West)]
        public void RightCommandWithPlacementIsSuccessful(DirectionEnum existingDirection, DirectionEnum newDirection)
        {
            // Arrange
            var command = new CommandRequest(CommandEnum.Right);
            var currentPosition = new Position(3, 4, existingDirection);
            var newPosition = new Position(currentPosition.X, currentPosition.Y, newDirection);

            _robotRepositoryMock.Setup(x => x.GetPosition()).Returns(currentPosition);
            _robotRepositoryMock.Setup(x => x.UpdatePosition(It.IsAny<Position>())).Returns(true);

            var robotService = new RobotService(_robotRepositoryMock.Object);

            // Act
            var result = robotService.ProcessCommand(command);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(newDirection, result.Position.Direction);
        }
        #endregion

        #region Report Command Tests
        [Test]
        public void ReportCommandWithPlacementIsSuccessful()
        {
            // Arrange
            var command = new CommandRequest(CommandEnum.Report);
            var currentPosition = new Position(3, 4, DirectionEnum.North);
            
            _robotRepositoryMock.Setup(x => x.GetPosition()).Returns(currentPosition);

            var robotService = new RobotService(_robotRepositoryMock.Object);

            // Act
            var result = robotService.ProcessCommand(command);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(currentPosition, result.Position);
        }
        #endregion
    }
}
