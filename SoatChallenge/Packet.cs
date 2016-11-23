namespace SoatChallenge
{
    /// <summary>A packet to be delivered</summary>
    public class Packet : Cell
    {
        /// <summary>Initializes a new instance of the <see cref="Packet"/> class.</summary>
        /// <param name="row">Packet row position</param>
        /// <param name="column">Packet column position</param>
        public Packet(int row, int column) : base(row, column)
        {
            this.CurrentState = State.Pending;
            this.Distance = 0;
        }

        /// <summary>packet state</summary>
        public enum State
        {
            /// <summary>packet is pending for a shipper</summary>
            Pending = 0,

            /// <summary>packet is willing to be assigned</summary>
            Willing = 1,

            /// <summary>packet missed last attempt to get a route</summary>
            Missing = 2,

            /// <summary>packet is assigned to a shipper</summary>
            Assigned = 3,

            /// <summary>packet is delivered</summary>
            Delivered = 4,
        }

        /// <summary>Gets current cell state</summary>
        public State CurrentState { get; internal set; }

        /// <summary>Gets the number of round before shipping can happen</summary>
        public int Distance { get; internal set; }

        /// <summary>Gets a string representation of the current object</summary>
        /// <returns>this as <see cref="string"/></returns>
        public override string ToString()
        {
            return Write.Invariant($"R{this.Row}C{this.Column}({this.Distance})");
        }
    }
}