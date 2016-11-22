namespace SoatChallenge
{
    /// <summary>Represent a cell, packet or routeCell</summary>
    public interface ICell
    {
        /// <summary>Gets or sets cell columns</summary>
        int Column { get; set; }

        /// <summary>Gets or sets cell row</summary>
        int Row { get; set; }
    }
}