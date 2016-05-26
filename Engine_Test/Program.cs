using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BHoM.Geometry;
using BHoM.Generic;
using BHoM_Engine.Graph;

namespace Engine_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Point> points = new List<Point>();
            points.Add(new Point(0, 0, 0));
            points.Add(new Point(0, 2, 0));
            points.Add(new Point(0, 6, 0));
            points.Add(new Point(4, 4, 0));
            points.Add(new Point(4, 6, 0));

            Graph<Point> graph = new Graph<Point>();
            List<GraphNode<Point>> nodes = new List<GraphNode<Point>>();
            foreach (Point pt in points)
                nodes.Add(graph.AddNode(pt));  
            for (int i = 1; i < points.Count; i++)
                graph.AddUndirectedLink(points[i - 1], points[i], points[i-1].DistanceTo(points[i]));
            graph.AddUndirectedLink(points[1], points[3], points[1].DistanceTo(points[3]));
            graph.AddUndirectedLink(points[2], points[4], points[2].DistanceTo(points[4]));

            GraphNavigator<Point> navigator = new GraphNavigator<Point>(graph);
            List<GraphNode<Point>> path = navigator.GetPath(nodes[0], nodes.Last(), PointDist);
        }

        static double PointDist(Point pt1, Point pt2)
        {
            return pt1.DistanceTo(pt2);
        }
    }
}
