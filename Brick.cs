namespace Brickwork
{
    public class Brick
    {
        public int Line { get; set; }
        public int Column { get; set; }
        public int? Number { get; set; }

        public bool IsHorizontal { get; set; }

        public bool IsFirstBrick { get; set; }
    }
}
