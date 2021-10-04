namespace ToyRobot.Models
{
    public record CommandRequest (CommandEnum Command, Position? Position = null);
}