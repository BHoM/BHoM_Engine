using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BHG = BHoM.Geometry;
using BHB = BHoM.Base;
using BHoM.Structural.Elements;
using BHoM.Structural.Properties;
using ModelLaundry_Engine;

namespace Engine_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestAudio();
            Console.Read();
        }

        static void TestProject()
        {
            /*Project project = Project.ActiveProject;

            List<Point> points = new List<Point>();
            points.Add(new Point(0, 0, 0));
            points.Add(new Point(0, 1, 0));
            points.Add(new Point(1, 1, 0));
            points.Add(new Point(0, 0, 0));

            Group<Curve> edges = new Group<Curve>();
            for (int i = 1; i < points.Count; i++)
                edges.Add(new Line(points[i - 1], points[i]));

            Panel panel = new Panel(edges);
            panel.PanelProperty = new ConstantThickness("test", 0.25, PanelType.Wall);

            project.AddObject(panel);

            string json = project.ToJSON();
            project.Clear();

            Project project2 = Project.FromJSON(json);*/
        }

       /* static void TestVideo()
        {
            string videoFile = @"C:\Users\adecler\Documents\Projects\StadiaCrowdAnalysis\InputVideos\Fan Cam- Preston North End_mpeg4.avi";
            MachineLearning_Engine.MotionLevelAnalyser analyser = new MachineLearning_Engine.MotionLevelAnalyser();

            MachineLearning_Engine.MotionLevelAnalyser.Config config = new MachineLearning_Engine.MotionLevelAnalyser.Config();
            config.FrameStep = 10;
            config.OutFolder = @"C:\Users\adecler\Documents\Projects\StadiaCrowdAnalysis\Results\Video_01";
            config.NbRows = 3;
            config.NbColumns = 1;

            Dictionary<int, List<double>> result = analyser.Run(videoFile, config).Result;
        }*/

        /*static void TestAudio()
        {
            string videoFile = @"C:\Users\adecler\Documents\Projects\StadiaCrowdAnalysis\InputVideos\Fan Cam- Preston North End_mpeg4.wav";
            MachineLearning_Engine.SoundLevelAnalyser analyser = new MachineLearning_Engine.SoundLevelAnalyser();

            MachineLearning_Engine.SoundLevelAnalyser.Config config = new MachineLearning_Engine.SoundLevelAnalyser.Config();
            config.OutFolder = @"C:\Users\adecler\Documents\Projects\StadiaCrowdAnalysis\Results\Video_01";

            Dictionary<int, double> result = analyser.Run(videoFile, config).Result;
        }*/

        /*static void TestPanelVSnap()
        {
            // Test panel snapping
            string panelJson = "{ \"Type\":\"BHoM.Structural.Panel\",\"Primitive\":\"BHoM.Structural.Panel; BHoM; Version=1.0.0.0; Culture=neutral; PublicKeyToken=null\",\"Properties\":{ \"Edges\":{ \"Primitive\":\"group\",\"groupType\":\"BHoM.Geometry.Curve\",\"group\":[{\"Primitive\":\"line\",\"start\":[24.5778681994532,-2.74803939183246,45.315],\"end\":[29.5556354116369,-9.05481358976734,45.315]},{\"Primitive\":\"line\",\"start\":[29.5556354116369,-9.05481358976734,45.315],\"end\":[29.5556354116369,-9.05481358976734,48.065]},{\"Primitive\":\"line\",\"start\":[29.5556354116369,-9.05481358976734,48.065],\"end\":[24.5778681994532,-2.74803939183246,48.065]},{\"Primitive\":\"line\",\"start\":[24.5778681994532,-2.74803939183246,48.065],\"end\":[24.5778681994532,-2.74803939183246,45.315]}]},\"ThicknessProperty\":\"ab645fdd-b100-4d87-ab16-84ab6e053218\",\"BHoM_Guid\":\"ea18e6ab-1cae-4cbd-9f24-05c9f608f486\",\"CustomData\":{\"RevitId\":804997,\"RevitType\":\"Wall\"}}}";
            Panel panel = BHoMObject.FromJSON(panelJson, Project.ActiveProject) as Panel;

            panelJson = "{ \"Type\":\"BHoM.Structural.Panel\",\"Primitive\":\"BHoM.Structural.Panel; BHoM; Version=1.0.0.0; Culture=neutral; PublicKeyToken=null\",\"Properties\":{ \"Edges\":{ \"Primitive\":\"group\",\"groupType\":\"BHoM.Geometry.Curve\",\"group\":[{\"Primitive\":\"line\",\"start\":[28.8385630649439,-7.90417884756682,45.315],\"end\":[31.0639227821543,-6.14776228848485,45.315]},{\"Primitive\":\"line\",\"start\":[31.0639227821543,-6.14776228848485,45.315],\"end\":[31.0639227821543,-6.14776228848485,48.065]},{\"Primitive\":\"line\",\"start\":[31.0639227821543,-6.14776228848485,48.065],\"end\":[28.8385630649439,-7.90417884756682,48.065]},{\"Primitive\":\"line\",\"start\":[28.8385630649439,-7.90417884756682,48.065],\"end\":[28.8385630649439,-7.90417884756682,45.315]}]},\"ThicknessProperty\":\"460c4041-6c21-41cb-9b68-0e34250ee406\",\"BHoM_Guid\":\"6c21eefa-8c84-4256-8979-f232bc3e8917\",\"CustomData\":{\"RevitId\":804999,\"RevitType\":\"Wall\"}}}";
            Panel panel2 = BHoMObject.FromJSON(panelJson, Project.ActiveProject) as Panel;

            List<double> heights = new List<double>();
            heights.Add(45);
            ModelLaundry_Engine.Snapping.VerticalSnapToHeight(panel, heights, 1);
            panel = ModelLaundry_Engine.Util.HorizontalExtend(panel, 1) as Panel;
            panel2 = ModelLaundry_Engine.Util.HorizontalExtend(panel2, 1) as Panel;

            List<object> refCurves = new List<object>();
            foreach (Curve curve in panel2.External_Contours)
            refCurves.Add(curve);
            ModelLaundry_Engine.Snapping.HorizontalSnapToShape(panel, refCurves, 1);

            Console.WriteLine("Done");
        }*/

        /*static void TestLineVSnap()
        { 
            // Test Line and Bar snapping
            Point pt1 = new Point(0 , 0, 0.5);
            Point pt2 = new Point(0, 0, 4.5);

            List<double> heights = new List<double>();
            heights.Add(0);
            heights.Add(5);

            Line line = new Line(pt1, pt2);
            Curve r1 = Snapping.VerticalSnapToHeight(line, heights, 0.7);
            Console.WriteLine("Line: [{0} - {1}", r1.StartPoint.ToString(), r1.EndPoint.ToString());

            Bar bar = new Bar(pt1, pt2);
            Bar r2 = Snapping.VerticalSnapToHeight(bar, heights, 0.7) as Bar;
            Console.WriteLine("Line: [{0} - {1}", r2.StartPoint.ToString(), r2.EndPoint.ToString());

            Console.WriteLine("Done");
        }*/

        /*static void TestGraph()
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

            Graph_Engine.GraphNavigator<Point> navigator = new Graph_Engine.GraphNavigator<Point>(graph);
            List<GraphNode<Point>> path = navigator.GetPath(nodes[0], nodes.Last(), PointDist);
        }*/

        /*static double PointDist(Point pt1, Point pt2)
        {
            return pt1.DistanceTo(pt2);
        }*/
    }
}
