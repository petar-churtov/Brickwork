using System.Collections.Generic;

namespace Brickwork
{
    public class Layer
    {
        public Layer()
        {
            Bricks = new List<Brick>();
        }

        public List<Brick> Bricks { get; set; }
    }
}
