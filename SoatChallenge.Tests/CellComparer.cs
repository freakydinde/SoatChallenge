namespace SoatChallenge.Tests
{
    using System.Collections.Generic;

    /// <summary>compare 2 cells</summary>
    public class CellComparer : Comparer<Cell>
    {
        /// <summary>Override the cell comparer</summary>
        /// <param name="x">this cell</param>
        /// <param name="y">other cell</param>
        /// <returns>this.Cell == that.Cell</returns>
        public override int Compare(Cell x, Cell y)
        {
            if (x != null && y != null)
            {
                if (x.Column == y.Column && x.Row == y.Row)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                if (x == null && y == null)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
        }
    }
}