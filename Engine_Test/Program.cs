using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BHoM.Geometry;
using BHoM.Global;
using BHoM.Generic;
using BHoM_Engine.Graph;
using BHoM.Structural;

namespace Engine_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            TestMongo();

            Console.Read();
        }

        static void TestMongo()
        {
            // Create a fake project
            List<Node> nodes = new List<Node>();
            for (int i = 0; i < 10; i++)
                nodes.Add(new Node(i, 2, 3));

            List<Bar> bars = new List<Bar>();
            for (int i = 1; i < 10; i++)
                bars.Add(new Bar(nodes[i - 1], nodes[i]));

            var project = Project.ActiveProject;
            foreach (Node node in nodes)
                project.AddObject(node);
            foreach (Bar bar in bars)
                project.AddObject(bar);

            // Create database
            var mongo = new BHoM_Engine.Databases.Mongo.MongoLink();
            Console.WriteLine("Database link created");

            // Add Objects to the dtabase
            //mongo.SaveObjects(project.Objects);
            //Console.WriteLine("Objects added to the database");

            // Get all object from database
            IEnumerable<BHoM.Global.BHoMObject> objects = mongo.GetObjects("{}");
            Console.WriteLine("Objects obtained from the database: {0}", objects.Count());
        }

        static void TestGraph()
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
                graph.AddUndirectedLink(points[i - 1], points[i], points[i - 1].DistanceTo(points[i]));
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
