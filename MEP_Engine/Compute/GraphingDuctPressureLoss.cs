using System;
using System.Collections.Generic;
using System.Text;

using BH.oM.Geometry;
using System.Linq;

using BH.Engine.Geometry;

namespace BH.Engine.MEP
{
    public static partial class Compute
    {

        public static List<DuctConnection> GraphMeUp(List<Line> ducts)
        {
            List<Point> connectionPoints = ducts.Select(x => x.Start).ToList();
            connectionPoints.AddRange(ducts.Select(x => x.End));

            List<DuctConnection> connections = new List<DuctConnection>();

            foreach(Point p in connectionPoints)
            {
                DuctConnection connection = new DuctConnection();
                connection.Node = p;

                var allDuctsConnected = ducts.Where(x => x.Start == p).ToList();
                connection.Connections = allDuctsConnected.Select(x =>
                {
                    return new Conn()
                    {
                        ConnectionPoint = x.End,
                        ConnectionDistance = x.End.Distance(p),
                    };
                }).ToList();

                allDuctsConnected = ducts.Where(x => x.End == p).ToList();
                connection.Connections.AddRange(allDuctsConnected.Select(x =>
                {
                    return new Conn()
                    {
                        ConnectionPoint = x.Start,
                        ConnectionDistance = x.Start.Distance(p),
                    };
                }));

                connections.Add(connection);
            }

            return connections;
        }

        public static List<Line> ConnectedGraph(List<DuctConnection> connections, Point start, Point end)
        {
            List<DuctConnection> possibleOptions = new List<DuctConnection>(connections);

            var currentConnection = possibleOptions.Where(x => x.Node == start).FirstOrDefault();

            List<DuctConnection> route = new List<DuctConnection>();
            route.Add(currentConnection);

            Dictionary<DuctConnection, DuctConnection> cameFrom = new Dictionary<DuctConnection, DuctConnection>();

            Dictionary<DuctConnection, double> gScore = new Dictionary<DuctConnection, double>();
            foreach (DuctConnection connection in possibleOptions)
                gScore.Add(connection, 1e10);

            //possibleOptions.Remove(currentConnection);
            gScore[currentConnection] = 0;

            Dictionary<DuctConnection, double> fScore = new Dictionary<DuctConnection, double>(gScore);

            while(route.Count > 0)
            {
                double currentVal = 1e10;
                DuctConnection current = null;
                foreach(var r in route)
                {
                    var x = fScore[r];
                    if(x < currentVal)
                    {
                        currentVal = x;
                        current = r;
                    }
                }

                if (current.Node == end)
                    return ReconstructPath(cameFrom, current);

                route.Remove(current);
                foreach(var neigh in current.Connections)
                {
                    double tentativeGScore = gScore[current] + neigh.ConnectionDistance;

                    var connNeigh = possibleOptions.Where(x => x.Node == neigh.ConnectionPoint).FirstOrDefault();

                    if(tentativeGScore < gScore[connNeigh])
                    {
                        cameFrom.Add(connNeigh, current);
                        gScore[connNeigh] = tentativeGScore;
                        fScore[connNeigh] = tentativeGScore + neigh.ConnectionDistance;
                        if (!route.Contains(connNeigh))
                            route.Add(connNeigh);
                    }
                }
            }

            return null;
        }

        public static List<Line> ReconstructPath(Dictionary<DuctConnection, DuctConnection> cameFrom, DuctConnection current)
        {
            List<DuctConnection> path = new List<DuctConnection>();
            path.Add(current);

            while(cameFrom.Keys.Contains(current))
            {
                current = cameFrom[current];// cameFrom[0];
                if (current == null)
                    break;
                path.Add(current);
                //cameFrom.Remove(current);
            }

            List<Line> lines = new List<Line>();
            for(int x = 0; x < path.Count - 1; x++)
            {
                lines.Add(new Line()
                {
                    Start = path[x].Node,
                    End = path[x + 1].Node,
                });
            }

            lines.Reverse();
            return lines;
        }

        public class DuctConnection
        {
            public Point Node { get; set; }
            public List<Conn> Connections { get; set; }
        }

        public class Conn
        {
            public Point ConnectionPoint { get; set; }
            public double ConnectionDistance { get; set; }
        }
    }
}
