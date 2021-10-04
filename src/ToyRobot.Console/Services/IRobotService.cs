using ToyRobot.Models;

namespace ToyRobot.Services
{
    public interface IRobotService
    {
        CommandResult ProcessCommand(CommandRequest command);
    }
}
