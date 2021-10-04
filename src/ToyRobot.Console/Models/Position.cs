namespace ToyRobot.Models
{
    public class Position
    {
        public Position(int x, int y, DirectionEnum? direction = null)
        {
            X = x;
            Y = y;
            Direction = direction;
        }

        public int X { get; init; }
        public int Y { get; init; }
        public DirectionEnum? Direction { get; set; }
    }
}
