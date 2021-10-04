using ToyRobot.Models;

namespace ToyRobot.Repositories
{
    public interface IRobotRepository
    {
        Position GetPosition();
        bool UpdatePosition(Position position);
    }
}
