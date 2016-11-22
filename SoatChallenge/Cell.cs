namespace SoatChallenge
{
    /// <summary>Represent a grid cell</summary>
    public class Cell : ICell
    {
        /// <summary>Initializes a new instance of the <see cref="Cell"/> class.</summary>
        /// <param name="row">Cell row position</param>
        /// <param name="column">Cell column position</param>
        public Cell(int row, int column)
        {
            this.Row = row;
            this.Column = column;
        }

        /// <summary>Gets or sets cell column position</summary>
        public int Column { get; set; }

        /// <summary>Gets or sets cell row position</summary>
        public int Row { get; set; }

        /// <summary>Gets a string representation of the current object</summary>
        /// <returns>this as <see cref="string"/></returns>
        public override string ToString()
        {
            return Write.Invariant($"R{this.Row}C{this.Column}");
        }
    }
}