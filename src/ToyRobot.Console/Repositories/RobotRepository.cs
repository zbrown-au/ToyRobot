using Microsoft.Extensions.Caching.Memory;
using ToyRobot.Models;

namespace ToyRobot.Repositories
{
    public class RobotRepository : IRobotRepository
    {
        private const string POSITION_KEY = "Position";

        private readonly IMemoryCache _cache;
        
        public RobotRepository (IMemoryCache cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// Get current position of the robot
        /// </summary>
        public Position GetPosition()
        {
            if (_cache.TryGetValue(POSITION_KEY, out Position position))
            {
                return position;
            }

            return null;
        }

        /// <summary>
        /// Update the position of the robot
        /// </summary>
        public bool UpdatePosition(Position position)
        {
            _cache.Set(POSITION_KEY, position);
            return true;
        }
    }
}
