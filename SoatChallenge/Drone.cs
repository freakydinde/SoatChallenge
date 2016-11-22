namespace SoatChallenge
{
    using System.Collections.Generic;

    /// <summary>Type representing a delivery drone</summary>
    public class Drone
    {
        private Grid grid;
        private List<int> moves;

        /// <summary>Initializes a new instance of the <see cref="Drone"/> class.</summary>
        /// <param name="id">drone identifier</param>
        /// <param name="droneGrid">world Path grid</param>
        public Drone(int id, Grid droneGrid)
        {
            // case where droneGrid is null should never happen anyway..
            this.grid = droneGrid ?? new Grid(0, 0, null, this.Position);
            this.Position = this.grid.StartCell ?? new Cell(0, 0);

            this.Id = id;
            this.Round = 0;

            this.CurrentState = State.Pending;
            this.Route = null;
        }

        /// <summary>drone direction</summary>
        public enum Direction
        {
            /// <summary>the drone does not move</summary>
            Stay = 0,

            /// <summary>done move left</summary>
            Left = 1,

            /// <summary>drone move up</summary>
            Up = 2,

            /// <summary>drone move right</summary>
            Right = 3,

            /// <summary>drone move down</summary>
            Down = 4
        }

        /// <summary>Cell state</summary>
        public enum State
        {
            /// <summary>drone is pending for instruction</summary>
            Pending = 0,

            /// <summary>drone got a route and is ready to ship</summary>
            Ready = 1,

            /// <summary>drone is currently shipping</summary>
            Shipping = 2,

            /// <summary>drone can't move anymore</summary>
            Stopped = 3,
        }

        /// <summary>Gets the maximun number of packets a drone can hold</summary>
        public static int MaxPackets { get; internal set; }

        /// <summary>Gets drone current state, Pending, Shipping or Stopped</summary>
        public State CurrentState { get; internal set; }

        /// <summary>Gets drone identifier</summary>
        public int Id { get; private set; }

        /// <summary>Gets a string representing moves series</summary>
        public string MoveString
        {
            get
            {
                if (this.moves.Count > 0)
                {
                    return string.Join(" ", this.moves);
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>Gets drone current position</summary>
        public Cell Position { get; private set; }

        /// <summary>Gets current round number</summary>
        public int Round { get; private set; }

        /// <summary>Gets drone route</summary>
        public Route Route { get; private set; }

        /// <summary>Gets drone route</summary>
        public IEnumerator<RouteCell> RouteCells { get; private set; }

        /// <summary>Gets direction from string using switch statement (faster than Enum.TryParse)</summary>
        /// <param name="direction">direction as string</param>
        /// <returns>direction as Drone.Direction</returns>
        public static Direction DirectionFromString(string direction)
        {
            // case : default
            Direction droneDirection = Direction.Stay;

            switch (direction)
            {
                case "Left":
                    droneDirection = Direction.Left;
                    break;

                case "Up":
                    droneDirection = Direction.Up;
                    break;

                case "Right":
                    droneDirection = Direction.Right;
                    break;

                case "Down":
                    droneDirection = Direction.Down;
                    break;
            }

            return droneDirection;
        }

        /// <summary>Gets the maximun number of packets a drone can hold to reach that distance</summary>
        /// <param name="distance">distance to travel</param>
        /// <returns>maximun packets number</returns>
        public static int MaxPacketsToReachDistance(int distance)
        {
            int maxPackets = 0;

            for (int i = 0; i <= Drone.MaxPackets; i++)
            {
                int autonomy = Delivery.Autonomy(i);

                if (autonomy >= distance)
                {
                    maxPackets = i;
                }
            }

            return maxPackets;
        }

        /// <summary>perform next move from routeCells move list</summary>
        public void NextMove()
        {
            this.Round++;

            if (this.CurrentState == State.Shipping)
            {
                if (this.RouteCells.MoveNext())
                {
                    this.Move(this.RouteCells.Current);
                }
                else
                {
                    this.Stop();
                }
            }
            else
            {
                Write.Trace($"drone {this.Id} stay in {this.Position}");
                this.moves.Add((int)Drone.Direction.Stay);
            }
        }

        /// <summary>reset current route, set state back to pending</summary>
        public void Reset()
        {
            this.Route.Reset();
            this.Route = null;
            this.CurrentState = State.Pending;
            this.moves = new List<int>();
            this.Round = 0;
        }

        /// <summary>define a route for this drone, add route packet</summary>
        /// <param name="route">route to set</param>
        public void SetRoute(Route route)
        {
            Write.Trace($"set route to drone {this.Id} : {route}");

            if (route != null)
            {
                this.Route = route;
                this.RouteCells = this.Route.Cells.GetEnumerator();
                this.CurrentState = State.Ready;
            }
        }

        /// <summary>start current drone</summary>
        public void Start()
        {
            Write.Trace($"drone {this.Id} start");

            this.Round = 0;

            this.moves = new List<int>();

            if (this.Route != null)
            {
                this.moves.Add(this.Route.PacketsCount);

                this.CurrentState = State.Shipping;
            }
            else
            {
                this.moves.Add(0);
            }
        }

        /// <summary>Gets a string representation of the current object</summary>
        /// <returns>this as <see cref="string"/></returns>
        public override string ToString()
        {
            return Write.Invariant($"Id:{this.Id} State:{this.CurrentState} Position:{this.Position} Round:{this.Round} PacketNumber:{this.Route.PacketsCount} MaxPackets:{Drone.MaxPackets} Route:({this.Route})");
        }

        /// <summary>Move drone on the grid</summary>
        /// <param name="destination">move direction, up, down, left, right</param>
        private void Move(RouteCell destination)
        {
            Write.Trace($"moving drone {this.Id} to {destination}");

            this.Position = new Cell(destination.Row, destination.Column);

            if (this.grid.SetPacketState(this.Position, Packet.State.Delivered))
            {
                Write.Trace($"{this.Position} packet delivered");
            }

            this.moves.Add((int)destination.Direction);
        }

        /// <summary>stop current drone, no more move allowed</summary>
        private void Stop()
        {
            Write.Trace($"drone {this.Id} stop");

            this.CurrentState = State.Stopped;
            this.moves.Add((int)Drone.Direction.Stay);
        }
    }
}