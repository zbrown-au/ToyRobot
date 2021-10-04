namespace ToyRobot.Models
{
    public record CommandResult(Position Position, bool IsSuccess, string ErrorMessage);
}
