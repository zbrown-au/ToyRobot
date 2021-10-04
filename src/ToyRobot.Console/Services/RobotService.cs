using System;
using ToyRobot.Models;
using ToyRobot.Repositories;

namespace ToyRobot.Services
{
    public class RobotService : IRobotService
    {
        private const int MIN_X_POSITION = 0;
        private const int MIN_Y_POSITION = 0;
        private const int MAX_X_POSITION = 5;
        private const int MAX_Y_POSITION = 5;

        public static readonly string NOT_PLACED_ERROR = "Robot is not placed";
        public static readonly string NOT_VALID_ERROR = "Position was not valid";

        private readonly IRobotRepository _robotRepository;

        public RobotService(IRobotRepository robotRepository)
        {
            _robotRepository = robotRepository;
        }

        /// <summary>
        /// Process a command for the robot
        /// </summary>
        public CommandResult ProcessCommand(CommandRequest command)
        {
            var currentPosition = _robotRepository.GetPosition();

            return command.Command switch
            {
                CommandEnum.Place => Place(currentPosition, command.Position),
                CommandEnum.Move => Move(currentPosition),
                CommandEnum.Left => Left(currentPosition),
                CommandEnum.Right => Right(currentPosition),
                CommandEnum.Report => new CommandResult(currentPosition, currentPosition != null, currentPosition != null ? string.Empty : NOT_PLACED_ERROR),
                _ => throw new NotSupportedException()
            };
        }

        /// <summary>
        /// Place the robot at a specified set of co-ordinates and in a direction
        /// </summary>
        private CommandResult Place(Position currentPosition, Position newPosition)
        {
            var isValid = IsValidPlacement(newPosition.X, newPosition.Y) && (newPosition.Direction.HasValue || currentPosition != null);
            var success = false;

            // Set direction to current direction if not supplied
            if (isValid && !newPosition.Direction.HasValue)
            {
                newPosition.Direction = currentPosition.Direction;
            }
            
            // Update position if valid
            if (isValid)
            {
                success = _robotRepository.UpdatePosition(newPosition);
            }

            var errorMessage = isValid ? string.Empty : NOT_VALID_ERROR;

            return new CommandResult(newPosition, success, errorMessage);
        }

        /// <summary>
        /// Move the robot one space in the current direction
        /// </summary>
        private CommandResult Move(Position currentPosition)
        {
            Position newPosition = null;
            var success = false;
            var errorMessage = string.Empty;

            // Update position if position is set
            if (currentPosition != null)
            {
                newPosition = currentPosition.Direction switch
                {
                    DirectionEnum.North => new Position(currentPosition.X, currentPosition.Y+1, currentPosition.Direction),
                    DirectionEnum.East => new Position(currentPosition.X+1, currentPosition.Y, currentPosition.Direction),
                    DirectionEnum.South => new Position(currentPosition.X, currentPosition.Y-1, currentPosition.Direction),
                    DirectionEnum.West => new Position(currentPosition.X-1, currentPosition.Y, currentPosition.Direction),
                    _ => currentPosition
                };

                // Update position if valid
                if (IsValidPlacement(newPosition.X, newPosition.Y))
                {
                    success = _robotRepository.UpdatePosition(newPosition);
                }
                else
                {
                    errorMessage = NOT_VALID_ERROR;
                }                
            }
            else
            {
                errorMessage = NOT_PLACED_ERROR;
            }

            return new CommandResult(newPosition, success, errorMessage);
        }

        /// <summary>
        /// Turn the robot left
        /// </summary>
        private CommandResult Left(Position currentPosition)
        {
            Position newPosition = null;
            var success = false;
            var errorMessage = string.Empty;

            // Update direction if position is set
            if (currentPosition != null)
            {
                var newDirection = currentPosition.Direction switch
                {
                    DirectionEnum.North => DirectionEnum.West,
                    DirectionEnum.East => DirectionEnum.North,
                    DirectionEnum.South => DirectionEnum.East,
                    DirectionEnum.West => DirectionEnum.South,
                    _ => currentPosition.Direction
                };

                newPosition = new Position(currentPosition.X, currentPosition.Y, newDirection);
                success = _robotRepository.UpdatePosition(newPosition);
            } 
            else 
            {
                errorMessage = NOT_PLACED_ERROR;
            }

            return new CommandResult(newPosition, success, errorMessage);
        }

        /// <summary>
        /// Turn the robot right
        /// </summary>
        private CommandResult Right(Position currentPosition)
        {            
            Position newPosition = null;
            var success = false;
            var errorMessage = string.Empty;

            // Update direction if position is set
            if (currentPosition != null)
            {
                var newDirection = currentPosition.Direction switch
                {
                    DirectionEnum.North => DirectionEnum.East,
                    DirectionEnum.East => DirectionEnum.South,
                    DirectionEnum.South => DirectionEnum.West,
                    DirectionEnum.West => DirectionEnum.North,
                    _ => currentPosition.Direction
                };

                newPosition = new Position(currentPosition.X, currentPosition.Y, newDirection);
                success = _robotRepository.UpdatePosition(newPosition);
            }
            else
            {
                errorMessage = NOT_PLACED_ERROR;
            }

            return new CommandResult(newPosition, success, errorMessage);
        }

        /// <summary>
        /// Check if placement co-ordinates are valid
        /// </summary>
        private bool IsValidPlacement(int? x, int? y)
        {
            return x.HasValue && x.Value >= MIN_X_POSITION && x.Value <= MAX_X_POSITION
                && y.HasValue && y.Value >= MIN_Y_POSITION && y.Value <= MAX_Y_POSITION;
        }
    }
}
