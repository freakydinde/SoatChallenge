namespace SoatChallenge
{
    /// <summary>Short path directions way and count</summary>
    public class Directions
    {
        /// <summary>Initializes a new instance of the <see cref="Directions"/> class.</summary>
        public Directions()
        {
            this.HorizontalCount = 0;
            this.VerticalCount = 0;
            this.Distance = 0;

            this.HorizontalDirection = Drone.Direction.Stay;
            this.VerticalDirection = Drone.Direction.Stay;
        }

        /// <summary>Gets or sets the distance from startcell to reachcell</summary>
        public int Distance { get; set; }

        /// <summary>Gets or sets the horizontal distance from startcell to reachcell</summary>
        public int HorizontalCount { get; set; }

        /// <summary>Gets or sets the horizontal direction from startcell to reachcell</summary>
        public Drone.Direction HorizontalDirection { get; set; }

        /// <summary>Gets the opposite horizontal direction from startcell to reachcell</summary>
        public Drone.Direction HorizontalOpposite
        {
            get
            {
                if (this.HorizontalDirection == Drone.Direction.Left)
                {
                    return Drone.Direction.Right;
                }
                else if (this.HorizontalDirection == Drone.Direction.Right)
                {
                    return Drone.Direction.Left;
                }
                else
                {
                    return Drone.Direction.Stay;
                }
            }
        }

        /// <summary>Gets or sets the vertical distance from startcell to reachcell</summary>
        public int VerticalCount { get; set; }

        /// <summary>Gets or sets the vertical direction from startcell to reachcell</summary>
        public Drone.Direction VerticalDirection { get; set; }

        /// <summary>Gets the opposite vertical direction from startcell to reachcell</summary>
        public Drone.Direction VerticalOpposite
        {
            get
            {
                if (this.VerticalDirection == Drone.Direction.Up)
                {
                    return Drone.Direction.Down;
                }
                else if (this.VerticalDirection == Drone.Direction.Down)
                {
                    return Drone.Direction.Up;
                }
                else
                {
                    return Drone.Direction.Stay;
                }
            }
        }

        /// <summary>Gets a string representation of the current object</summary>
        /// <returns>this as <see cref="string"/></returns>
        public override string ToString()
        {
            return Write.Invariant($"D{this.Distance} ({this.VerticalCount}{this.VerticalDirection} {this.HorizontalCount}{this.HorizontalDirection})");
        }
    }
}