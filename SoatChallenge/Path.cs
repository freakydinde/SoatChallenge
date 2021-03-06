﻿namespace SoatChallenge
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    /// <summary>A path is made to compute route</summary>
    public class Path
    {
        /// <summary>Initializes a new instance of the <see cref="Path"/> class.</summary>
        /// <param name="reachCell">Path reach cell</param>
        /// <param name="grid">grid containing all cell including startcell</param>
        public Path(ICell reachCell, Grid grid)
        {
            if (grid != null)
            {
                this.StartCell = grid.StartCell;
                this.Grid = grid;
            }

            if (reachCell != null)
            {
                // cast does not work with ToString()
                this.ReachCell = new Cell(reachCell.Row, reachCell.Column);
                this.Directions = this.PathDirections(this.StartCell, this.ReachCell);
            }

            // Write.Trace($"new Path : {this}");
        }

        /// <summary>Initializes a new instance of the <see cref="Path"/> class.</summary>
        /// <param name="reachCell">Path reach cell</param>
        /// <param name="grid">grid containing all cell including startcell</param>
        /// <param name="startCell">path start cell (if different from grid start cell)</param>
        public Path(ICell reachCell, Grid grid, ICell startCell)
        {
            // cast does not work with ToString()
            this.StartCell = new Cell(startCell.Row, startCell.Column);
            this.ReachCell = new Cell(reachCell.Row, reachCell.Column);

            this.Grid = grid;

            this.Directions = this.PathDirections(this.StartCell, this.ReachCell);

            // Write.Trace($"new Path {this}");
        }

        /// <summary>Gets path directions</summary>
        public Directions Directions { get; private set; }

        /// <summary>Gets distance from startCell to reachCell</summary>
        public int Distance
        {
            get
            {
                return this.Directions.Distance;
            }
        }

        /// <summary>Gets grid host for this path</summary>
        public Grid Grid { get; private set; }

        /// <summary>Gets path reach cell</summary>
        public Cell ReachCell { get; private set; }

        /// <summary>Gets path start cell</summary>
        public Cell StartCell { get; private set; }

        /// <summary>Gets the closest packet as path</summary>
        /// <param name="startCell">path close from this cell</param>
        /// <returns>closest packet pending path</returns>
        public Path ClosestPendingPath(ICell startCell)
        {
            return (from i in this.Grid.PendingPackets select new Path(i, this.Grid, startCell)).OrderBy(x => x.Distance).FirstOrDefault();
        }

        /// <summary>Gets the fartest packet as path</summary>
        /// <param name="startCell">path far from this cell</param>
        /// <returns>farthest pending packet as path</returns>
        public Path FarthestPendingPath(ICell startCell)
        {
            return (from i in this.Grid.PendingPackets select new Path(i, this.Grid, startCell)).OrderByDescending(x => x.Distance).FirstOrDefault();
        }

        /// <summary>Gets first route corresponding to routeSpecs list</summary>
        /// <param name="routeSpecs">route specifications</param>
        /// <returns>Route corresponding to specs</returns>
        public Route MapBubbleRoute(Route.Specs routeSpecs)
        {
            return this.MapBubbleRoute(new List<Route.Specs> { routeSpecs });
        }

        /// <summary>Gets first route corresponding to routeSpecs list</summary>
        /// <param name="routesSpecs">route specifications list</param>
        /// <returns>Route corresponding to specs</returns>
        public Route MapBubbleRoute(IEnumerable<Route.Specs> routesSpecs)
        {
            Write.Trace($"map Bubble route, from {this.StartCell} to {this.ReachCell}");

            Route route = new Route(this.StartCell);

            // if startcell != reachCell first go to reach cell before starting bubbling
            if (this.StartCell.Row != this.ReachCell.Row || this.StartCell.Column != this.ReachCell.Column)
            {
                Path firstPath = new Path(this.ReachCell, this.Grid, this.StartCell);

                route = firstPath.GetRoute(routesSpecs);
                route?.AssignWilling();
            }

            while (route != null && route.PacketsCount < Drone.MaxPackets && this.Grid.PendingPacketsNumber > 0)
            {
                Path nextPath = this.ClosestPendingPath(route.ReachCell);

                if (nextPath != null)
                {
                    this.ReachCell = nextPath.ReachCell;

                    Route nextRoute = nextPath.GetRoute(routesSpecs);

                    if (nextRoute != null)
                    {
                        int nextDistance = route.Distance + nextRoute.Distance;
                        int nextPacketCount = route.PacketsCount + nextRoute.PacketsCount;

                        if (nextDistance <= Delivery.Autonomy(nextPacketCount) && nextPacketCount <= Drone.MaxPackets)
                        {
                            nextRoute.AssignWilling();

                            route.AddRoute(nextRoute);
                        }
                        else
                        {
                            Write.Trace($"break : nextDistance:{nextDistance} > Autonomy:{Delivery.Autonomy(nextPacketCount)} || nextPacketCount:{nextPacketCount} > MaxPackets:{Drone.MaxPackets}");

                            nextRoute.ResetWilling();
                            break;
                        }
                    }
                    else
                    {
                        Write.Trace($"break : route = null");
                        break;
                    }
                }
            }

            if (route != null && route.PacketsCount > 0)
            {
                return route;
            }
            else
            {
                return null;
            }
        }

        /// <summary>Gets route corresponding to routeSpecs</summary>
        /// <param name="routeSpecs">route specifications</param>
        /// <returns>Route corresponding to specs</returns>
        public Route MapRoute(Route.Specs routeSpecs)
        {
            return this.MapRoute(new List<Route.Specs>() { routeSpecs });
        }

        /// <summary>Gets first route corresponding to routeSpecs list</summary>
        /// <param name="routeSpecs">route specifications</param>
        /// <returns>Route corresponding to specs</returns>
        public Route MapRoute(IEnumerable<Route.Specs> routeSpecs)
        {
            Route route = this.GetRoute(routeSpecs);
            route?.AssignWilling();

            return route;
        }

        /// <summary>Gets a string representation of the current object</summary>
        /// <returns>this as <see cref="string"/></returns>
        public override string ToString()
        {
            return Write.Invariant($"StartCell:{this.StartCell} ReachCell:{this.ReachCell} Distance:{this.Distance}");
        }

        private List<RouteCell> AvailableDirections(Route route)
        {
            List<RouteCell> availableDirections = new List<RouteCell>();

            // create list with available results
            if (route.ReachCell.Row != this.ReachCell.Row)
            {
                availableDirections.Add(new RouteCell(route.ReachCell.Row - 1, route.ReachCell.Column, Drone.Direction.Up, route.Distance + 1, this.Grid));
                availableDirections.Add(new RouteCell(route.ReachCell.Row + 1, route.ReachCell.Column, Drone.Direction.Down, route.Distance + 1, this.Grid));
            }

            if (route.ReachCell.Column != this.ReachCell.Column)
            {
                availableDirections.Add(new RouteCell(route.ReachCell.Row, route.ReachCell.Column - 1, Drone.Direction.Left, route.Distance + 1, this.Grid));
                availableDirections.Add(new RouteCell(route.ReachCell.Row, route.ReachCell.Column + 1, Drone.Direction.Right, route.Distance + 1, this.Grid));
            }

            // flatten collection to iterate while removing item
            IEnumerable<RouteCell> returnList = availableDirections.ToList();

            foreach (RouteCell cell in returnList)
            {
                if (cell.Column < 0)
                {
                    cell.Column = Grid.Columns;
                }
                else if (cell.Column > Grid.Columns)
                {
                    cell.Column = 0;
                }

                if (cell.Row < 0 || cell.Row > this.Grid.Rows)
                {
                    // delete cells outside grid
                    availableDirections.Remove(cell);
                }
                else if (cell.Row == this.StartCell.Row && cell.Column == this.StartCell.Column)
                {
                    // delete start cell
                    availableDirections.Remove(cell);
                }
            }

            return availableDirections;
        }

        // Ooii
        private RouteCell DodgeCell(RouteCell packetCell, IEnumerable<RouteCell> nextDirections, ref Route route, Route.Specs routeSpecs)
        {
            Packet packet = this.Grid.GetPacket(packetCell);

            IEnumerable<RouteCell> alternativeCells = from i in this.AvailableDirections(route) where !nextDirections.Where(x => x.Row != i.Row && x.Column != i.Column).Any() select i;

            Write.Trace($"packet {packet} route.Distance + 1 {route.Distance + 1} - packet.Distance {packet.Distance}) > 2");

            if (packet.Distance - (route.Distance + 1) > 2)
            {
                route.AddCell(packetCell, this.Grid);

                RouteCell routeCell = this.NextDirection(route, Route.Specs.All);
                RouteCell otherRouteCell = this.NextDirection(route, Route.Specs.All | Route.Specs.Alternative);

                route.RemoveLastCell();

                List<Path> paths = new List<Path>();

                if (routeCell != null)
                {
                    paths.Add(new Path(routeCell, this.Grid, alternativeCells.ElementAt(0)));
                    paths.Add(new Path(routeCell, this.Grid, alternativeCells.ElementAt(1)));
                }

                if (otherRouteCell != null)
                {
                    paths.Add(new Path(otherRouteCell, this.Grid, alternativeCells.ElementAt(0)));
                    paths.Add(new Path(otherRouteCell, this.Grid, alternativeCells.ElementAt(1)));
                }

                if (paths.Any())
                {
                    List<Route> routes = new List<Route>();

                    foreach (Path path in paths)
                    {
                        Route newRoute = path.MapRoute(new List<Route.Specs>() { Route.Specs.All, Route.Specs.All | Route.Specs.Alternative });

                        if (newRoute != null)
                        {
                            routes.Add(newRoute);
                        }
                    }

                    Route selectedRoute = (from i in routes orderby i.Distance select i).FirstOrDefault();

                    if (selectedRoute != null)
                    {
                        route.AddRoute(selectedRoute);
                        return this.NextDirection(route, routeSpecs);
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                // insert a wait step on route start
                return RouteCell.WaitCell(this.StartCell);
            }
        }

        private RouteCell FilterAvailableDirections(IEnumerable<RouteCell> nextDirections, Route route, Route.Specs routeSpecs)
        {
            IEnumerable<RouteCell> returnList = null;

            // filter available directions according to route specs
            if (nextDirections.Any())
            {
                if (routeSpecs.HasFlag(Route.Specs.Route))
                {
                    // return packet cells if available, otherwise route cells
                    returnList = from i in nextDirections where i.IsPacket == true && i.WillBreakDelivery == false select i;

                    if (!returnList.Any())
                    {
                        returnList = from i in nextDirections where i.IsRoute == true && i.IsPacket == false select i;
                    }

                    if (!returnList.Any())
                    {
                        returnList = from i in nextDirections where i.IsStartRoute == true && i.IsPacket == false select i;
                    }
                }
                else if (routeSpecs.HasFlag(Route.Specs.Free))
                {
                    // return free cells if available, otherwise route cells
                    returnList = from i in nextDirections where i.IsFree == true select i;

                    if (!returnList.Any())
                    {
                        returnList = from i in nextDirections where i.IsPacket == false || i.WillBreakDelivery == false select i;
                    }
                }
                else if (routeSpecs.HasFlag(Route.Specs.All))
                {
                    // return all availables directions
                    returnList = from i in nextDirections where i.IsPacket == false || i.WillBreakDelivery == false select i;
                }

                if ((returnList == null || !returnList.Any()) && (routeSpecs.HasFlag(Route.Specs.Dodge) == true || routeSpecs.HasFlag(Route.Specs.Wait)))
                {
                    if (routeSpecs.HasFlag(Route.Specs.Dodge) == true)
                    {
                        RouteCell packetCell = (from i in nextDirections where i.WillBreakDelivery == true orderby i.Distance select i).FirstOrDefault();

                        // try to dodge cell or wait if distance gap is lower than 3 step
                        return this.DodgeCell(packetCell, nextDirections, ref route, routeSpecs);
                    }
                    else if (routeSpecs.HasFlag(Route.Specs.Wait))
                    {
                        // insert a wait step on route start
                        return RouteCell.WaitCell(this.StartCell);
                    }
                }
                else
                {
                    if (routeSpecs.HasFlag(Route.Specs.Alternative) && returnList.Count() > 1)
                    {
                        // Alternative
                        returnList = returnList.Skip(1);
                    }
                    else
                    {
                        // Route | Free | All following specs
                        returnList = returnList.Take(1);
                    }
                }

                return returnList.FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        private Route GetRoute(IEnumerable<Route.Specs> routesSpecs)
        {
            foreach (Route.Specs currentRouteSpecs in routesSpecs)
            {
                Route route = new Route(this.StartCell);

                Route.Specs routeSpecs = currentRouteSpecs;

                Write.Trace($"map {routeSpecs} route, from {this.StartCell} to {this.ReachCell}");

                while ((route.ReachCell.Row != this.ReachCell.Row || route.ReachCell.Column != this.ReachCell.Column) && route != null)
                {
                    RouteCell nextDirection = this.NextDirection(route, routeSpecs);

                    if (nextDirection != null)
                    {
                        RouteCell nextDirectionCell = nextDirection;

                        route.AddCell(nextDirectionCell, this.Grid);

                        if (route.PacketsCount == route.MaxPackets && routeSpecs.HasFlag(Route.Specs.Route))
                        {
                            routeSpecs &= ~Route.Specs.Route;
                            routeSpecs |= Route.Specs.Free;
                        }
                        else if (route.PacketsCount > route.MaxPackets)
                        {
                            Write.Trace($"break : route.PacketsCount > route.MaxPackets {route}");

                            route.Reset();
                            route = null;

                            break;
                        }
                    }
                    else
                    {
                        Write.Trace($"break : next direction = null {route}");

                        route.Reset();
                        route = null;

                        break;
                    }
                }

                if (route != null)
                {
                    return route;
                }
            }

            return null;
        }

        private RouteCell NextDirection(Route route, Route.Specs routeSpecs)
        {
            List<RouteCell> availableDirections = this.AvailableDirections(route);

            // flatten collection to iterate while removing item
            IEnumerable<RouteCell> returnList = availableDirections.ToList();

            foreach (RouteCell cell in returnList)
            {
                if (cell.Row == this.ReachCell.Row && cell.Column == this.ReachCell.Column)
                {
                    // return reach cell only if available
                    Write.Trace($"reach cell is available direction : {cell}");

                    return cell;
                }

                if (routeSpecs.HasFlag(Route.Specs.Opposite) && (cell.Direction != this.Directions.VerticalDirection && cell.Direction != this.Directions.HorizontalOpposite))
                {
                    availableDirections.Remove(cell);
                }
                else if (!routeSpecs.HasFlag(Route.Specs.Opposite) && cell.Direction != this.Directions.VerticalDirection && cell.Direction != this.Directions.HorizontalDirection)
                {
                    availableDirections.Remove(cell);
                }
            }

            return this.FilterAvailableDirections(availableDirections, route, routeSpecs);
        }

        private Directions PathDirections(ICell startCell, ICell reachCell)
        {
            int distanceLeft = 0;
            int distanceRight = 0;

            Directions returnDirections = new Directions();

            if (startCell.Column < reachCell.Column)
            {
                distanceRight = reachCell.Column - startCell.Column;
                distanceLeft = startCell.Column + (this.Grid.Columns - reachCell.Column) + 1;
            }
            else if (startCell.Column > reachCell.Column)
            {
                distanceLeft = startCell.Column - reachCell.Column;
                distanceRight = reachCell.Column + (this.Grid.Columns - startCell.Column) + 1;
            }

            if (distanceLeft > distanceRight)
            {
                returnDirections.HorizontalCount = distanceRight;
                returnDirections.HorizontalDirection = Drone.Direction.Right;
            }
            else if (distanceLeft < distanceRight)
            {
                returnDirections.HorizontalCount = distanceLeft;
                returnDirections.HorizontalDirection = Drone.Direction.Left;
            }
            else if (distanceLeft == distanceRight)
            {
                if (distanceRight == 0)
                {
                    returnDirections.HorizontalCount = 0;
                    returnDirections.HorizontalDirection = Drone.Direction.Stay;
                }
                else
                {
                    returnDirections.HorizontalCount = distanceRight;
                    returnDirections.HorizontalDirection = Drone.Direction.Right;
                }
            }

            if (startCell.Row < reachCell.Row)
            {
                returnDirections.VerticalCount = reachCell.Row - startCell.Row;
                returnDirections.VerticalDirection = Drone.Direction.Down;
            }
            else if (startCell.Row > reachCell.Row)
            {
                returnDirections.VerticalCount = startCell.Row - reachCell.Row;
                returnDirections.VerticalDirection = Drone.Direction.Up;
            }
            else
            {
                returnDirections.VerticalCount = 0;
                returnDirections.VerticalDirection = Drone.Direction.Stay;
            }

            returnDirections.Distance = returnDirections.HorizontalCount + returnDirections.VerticalCount;

            return returnDirections;
        }
    }
}