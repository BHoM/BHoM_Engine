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
            TestProject();

            Console.Read();
        }

        static void TestProject()
        {
            Project project = Project.ActiveProject;

            List<Point> points = new List<Point>();
            points.Add(new Point(0, 0, 0));
            points.Add(new Point(0, 1, 0));
            points.Add(new Point(1, 1, 0));
            points.Add(new Point(0, 0, 0));

            Group<Curve> edges = new Group<Curve>();
            for (int i = 1; i < points.Count; i++)
                edges.Add(new Line(points[i - 1], points[i]));

            Panel panel = new Panel(edges);
            panel.ThicknessProperty = new ConstantThickness("test", 0.25);

            project.AddObject(panel);

            string json = project.ToJSON();
            project.Clear();

            Project project2 = Project.FromJSON(json);
        }

        static void TestVSnap()
        {
            string panelJson = "{ \"Type\":\"BHoM.Structural.Panel\",\"Primitive\":\"BHoM.Structural.Panel; BHoM; Version=1.0.0.0; Culture=neutral; PublicKeyToken=null\",\"Properties\":{ \"Edges\":{ \"Primitive\":\"group\",\"groupType\":\"BHoM.Geometry.Curve\",\"group\":[{\"Primitive\":\"line\",\"start\":[24.5778681994532,-2.74803939183246,45.315],\"end\":[29.5556354116369,-9.05481358976734,45.315]},{\"Primitive\":\"line\",\"start\":[29.5556354116369,-9.05481358976734,45.315],\"end\":[29.5556354116369,-9.05481358976734,48.065]},{\"Primitive\":\"line\",\"start\":[29.5556354116369,-9.05481358976734,48.065],\"end\":[24.5778681994532,-2.74803939183246,48.065]},{\"Primitive\":\"line\",\"start\":[24.5778681994532,-2.74803939183246,48.065],\"end\":[24.5778681994532,-2.74803939183246,45.315]}]},\"ThicknessProperty\":\"ab645fdd-b100-4d87-ab16-84ab6e053218\",\"BHoM_Guid\":\"ea18e6ab-1cae-4cbd-9f24-05c9f608f486\",\"CustomData\":{\"RevitId\":804997,\"RevitType\":\"Wall\"}}}";
            Panel panel = BHoMObject.FromJSON(panelJson) as Panel;

            panelJson = "{ \"Type\":\"BHoM.Structural.Panel\",\"Primitive\":\"BHoM.Structural.Panel; BHoM; Version=1.0.0.0; Culture=neutral; PublicKeyToken=null\",\"Properties\":{ \"Edges\":{ \"Primitive\":\"group\",\"groupType\":\"BHoM.Geometry.Curve\",\"group\":[{\"Primitive\":\"line\",\"start\":[28.8385630649439,-7.90417884756682,45.315],\"end\":[31.0639227821543,-6.14776228848485,45.315]},{\"Primitive\":\"line\",\"start\":[31.0639227821543,-6.14776228848485,45.315],\"end\":[31.0639227821543,-6.14776228848485,48.065]},{\"Primitive\":\"line\",\"start\":[31.0639227821543,-6.14776228848485,48.065],\"end\":[28.8385630649439,-7.90417884756682,48.065]},{\"Primitive\":\"line\",\"start\":[28.8385630649439,-7.90417884756682,48.065],\"end\":[28.8385630649439,-7.90417884756682,45.315]}]},\"ThicknessProperty\":\"460c4041-6c21-41cb-9b68-0e34250ee406\",\"BHoM_Guid\":\"6c21eefa-8c84-4256-8979-f232bc3e8917\",\"CustomData\":{\"RevitId\":804999,\"RevitType\":\"Wall\"}}}";
            Panel panel2 = BHoMObject.FromJSON(panelJson) as Panel;

            List<double> heights = new List<double>();
            heights.Add(45);
            BHoM_Engine.ModelLaundry.Snapping.VerticalEndSnap(panel.Edges, heights, 1);
            panel.Edges = BHoM_Engine.ModelLaundry.Util.HorizontalExtend(panel.Edges, 1);
            panel2.Edges = BHoM_Engine.ModelLaundry.Util.HorizontalExtend(panel2.Edges, 1);

            List<Curve> refCurves = new List<Curve>();
            refCurves.Add(panel2.External_Contour);
            BHoM_Engine.ModelLaundry.Snapping.HorizontalPointSnap(panel.Edges, refCurves, 1);

            Console.WriteLine("Done");
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
