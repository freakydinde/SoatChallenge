namespace SoatChallenge
{
    /// <summary>Cell containing a route direction and properties</summary>
    public class RouteCell : Cell
    {
        /// <summary>Initializes a new instance of the <see cref="RouteCell"/> class.</summary>
        /// <param name="row">Cell row position</param>
        /// <param name="column">Cell column position</param>
        /// <param name="direction">route cell direction</param>
        public RouteCell(int row, int column, Drone.Direction direction) : base(row, column)
        {
            this.Direction = direction;
        }

        /// <summary>Initializes a new instance of the <see cref="RouteCell"/> class.</summary>
        /// <param name="row">Cell row position</param>
        /// <param name="column">Cell column position</param>
        /// <param name="direction">route cell direction</param>
        /// <param name="grid">grid owner of this route</param>
        public RouteCell(int row, int column, Drone.Direction direction, Grid grid) : this(row, column, direction)
        {
            this.IsPacket = false;
            this.IsRoute = false;
            this.IsFree = false;

            if (grid != null)
            {
                this.IsPacket = grid.IsPacket(this);
                this.IsRoute = grid.IsRoute(this);
                this.IsFree = grid.IsFree(this);
                this.IsStartRoute = grid.IsStartRoute(this);
            }
        }

        /// <summary>Gets or sets cell direction from previous route cell</summary>
        public Drone.Direction Direction { get; set; }

        /// <summary>Gets or sets a value indicating whether the packet is assigned</summary>
        public bool IsAssigned { get; set; }

        /// <summary>Gets or sets a value indicating whether the cell does not cross grid routes (delimited by packets row and column)</summary>
        public bool IsFree { get; set; }

        /// <summary>Gets or sets a value indicating whether the cell contains a packet</summary>
        public bool IsPacket { get; set; }

        /// <summary>Gets or sets a value indicating whether the routeCells belongs to grid routes (delimited by packets row and column)</summary>
        public bool IsRoute { get; set; }

        /// <summary>Gets or sets a value indicating whether the routeCells belongs to grid start routes (delimited by startcell row and column)</summary>
        public bool IsStartRoute { get; set; }

        /// <summary>Gets or sets a value indicating whether the packet is planned to be delivered after route cell current distance</summary>
        public bool WillBreakDelivery { get; set; }

        /// <summary>create a routeCell from a startcell</summary>
        /// <param name="cell">startcell as <see cref="Cell"/></param>
        /// <returns>startcell as <see cref="RouteCell"/></returns>
        public static RouteCell WaitCell(Cell cell)
        {
            if (cell != null)
            {
                RouteCell routeCell = new RouteCell(cell.Row, cell.Column, Drone.Direction.Stay);
                return routeCell;
            }
            else
            {
                return null;
            }
        }

        /// <summary>Gets a string representation of the current object</summary>
        /// <returns>this as <see cref="string"/></returns>
        public override string ToString()
        {
            return Write.Invariant($"R{this.Row}C{this.Column}({this.Direction})");
        }
    }
}