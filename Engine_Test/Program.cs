﻿using System;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Geometry;
using BH.oM.Structural.Elements;
using System.Reflection;
using System.IO;
using BH.Engine.Base.Objects;
//using ModelLaundry_Engine;

namespace Engine_Test
{
    class Program
    {
        //static int nbSamples = 1000;
        //static List<Vector> testVectors = new List<Vector>();
        //static List<Line> testLines = new List<Line>();
        //static List<Polyline> testPoly = new List<Polyline>();


        static void Main(string[] args)
        {
            TestConstructorSpeed();
            //TestDynamicExtentionCost();
            //TestMethodExtraction();
            Console.Read();
        }

        /***************************************************/

        static void TestConstructorSpeed()
        {
            BH.Engine.Base.Compute.ExecutionSpeed(new List<IterFunction>
            {
                UseNodeConstructor,
                UseNodeCreator
            });
        }

        /***************************************************/

        static void UseNodeConstructor(int iter)
        {
            new Node(new Point(), "test");
        }

        /***************************************************/

        static void UseNodeCreator(int iter)
        {
            BH.Engine.Structure.Create.Node(new Point(), "test");
        }


        /***************************************************/

        static void TestMethodExtraction()
        {
            string folder = @"C:\Users\" + Environment.UserName + @"\AppData\Roaming\Grasshopper\Libraries\Alligator\";
            foreach (string file in Directory.GetFiles(folder))
            {
                if (file.EndsWith("_Engine.dll"))
                    Assembly.LoadFrom(file);
            }

            List<MethodInfo> methods = BH.Engine.Reflection.Query.GetBHoMMethodList();
            foreach (MethodInfo method in methods)
            {
                string def = method.DeclaringType.FullName + "." + method.Name + "(";
                if (method.GetParameters().Count() > 0)
                    def += method.GetParameters().Select(x => x.Name).Aggregate((x, y) => x + ", " + y);
                def += ")";
                Console.WriteLine(def);
            }
        }

        /***************************************************/

        //static void TestDynamicExtentionCost()
        //{
        //    // Testing for Bounding box of points
        //    testVectors = new List<Vector>();
        //    for (int i = 0; i < nbSamples; i++)
        //        testVectors.Add(new Vector(0, i, 0));

        //    Console.WriteLine("Testing for length of vectors...");
        //    BH.Engine.Testing.Speed.TestSpeed(new List<BH.Engine.Testing.IterFunction>
        //    {
        //        InClassLengthCall,
        //        DirectVectorLengthCall,
        //        PolymorphVectorLengthCall
        //    });

        //    // Testing for length of Lines
        //    testLines = new List<Line>();
        //    for (int i = 0; i < nbSamples; i++)
        //        testLines.Add(new Line(new Point(0, i, 0), new Point(i, 0, 0)));

        //    Console.WriteLine("\nTesting for length of lines...");
        //    BH.Engine.Testing.Speed.TestSpeed(new List<BH.Engine.Testing.IterFunction>
        //    {
        //        DirectLineLengthCall,
        //        PolymorphLineLengthCall
        //    });

        //    // Testing for length of Polylines with 20 CP
        //    int nbCP = 10;
        //    testPoly = new List<Polyline>();
        //    for (int i = 0; i < nbSamples; i++)
        //    {
        //        List<Point> pts = new List<Point>();
        //        for (int j = 0; j < nbCP; j++)
        //            pts.Add(new Point(nbSamples, nbCP, 0));
        //        testPoly.Add(new Polyline(pts));
        //    }

        //    Console.WriteLine("\nTesting for length of Polylines with " + nbCP + " CP...");
        //    BH.Engine.Testing.Speed.TestSpeed(new List<BH.Engine.Testing.IterFunction>
        //    {
        //        DirectPolyLineLengthCall,
        //        PolymorphPolyLineLengthCall
        //    });

        //    // Testing for length of Polylines with 50 CP
        //    nbCP = 30;
        //    testPoly = new List<Polyline>();
        //    for (int i = 0; i < nbSamples; i++)
        //    {
        //        List<Point> pts = new List<Point>();
        //        for (int j = 0; j < nbCP; j++)
        //            pts.Add(new Point(nbSamples, nbCP, 0));
        //        testPoly.Add(new Polyline(pts));
        //    }

        //    Console.WriteLine("\nTesting for length of Polylines with " + nbCP + " CP...");
        //    BH.Engine.Testing.Speed.TestSpeed(new List<BH.Engine.Testing.IterFunction>
        //    {
        //        DirectPolyLineLengthCall,
        //        PolymorphPolyLineLengthCall
        //    });

        //    // Testing for bounds of Polylines with X CP
        //    Console.WriteLine("\nTesting for bounds of Polylines with " + nbCP + " CP...");
        //    BH.Engine.Testing.Speed.TestSpeed(new List<BH.Engine.Testing.IterFunction>
        //    {
        //        DirectPolyLineBoundCall,
        //        PolymorphPolyLineBoundCall
        //    });
        //}

        ///***************************************************/

        //static void InClassLengthCall(int iter)
        //{
        //    testVectors[iter % nbSamples].Length();
        //}

        //static void DirectVectorLengthCall(int iter)
        //{
        //    Query._Length(testVectors[iter % nbSamples]);
        //}

        //static void PolymorphVectorLengthCall(int iter)
        //{
        //    Query.Length(testVectors[iter % nbSamples]);
        //}

        ///***************************************************/

        //static void DirectLineLengthCall(int iter)
        //{
        //    Query._Length(testLines[iter % nbSamples]);
        //}

        //static void PolymorphLineLengthCall(int iter)
        //{
        //    Query.Length(testLines[iter % nbSamples]);
        //}

        ///***************************************************/

        //static void DirectPolyLineLengthCall(int iter)
        //{
        //    Query._Length(testPoly[iter % nbSamples]);
        //}

        //static void PolymorphPolyLineLengthCall(int iter)
        //{
        //    Query.Length(testPoly[iter % nbSamples]);
        //}

        ///***************************************************/

        //static void DirectPolyLineBoundCall(int iter)
        //{
        //    Bounds._Bounds(testPoly[iter % nbSamples]);
        //}

        //static void PolymorphPolyLineBoundCall(int iter)
        //{
        //    Bounds.Bounds(testPoly[iter % nbSamples]);
        //}

        //static void TestProject()
        //{
        //    Project project = Project.ActiveProject;

        //    List<Point> points = new List<Point>();
        //    points.Add(new Point(0, 0, 0));
        //    points.Add(new Point(0, 1, 0));
        //    points.Add(new Point(1, 1, 0));
        //    points.Add(new Point(0, 0, 0));

        //    List<ICurve> edges = new List<ICurve>();
        //    for (int i = 1; i < points.Count; i++)
        //        edges.Add(new Line(points[i - 1], points[i]));

        //    Panel panel = new Panel(edges);
        //    panel.PanelProperty = new ConstantThickness("test", 0.25, PanelType.Wall);

        //    project.AddObject(panel);

        //    string json = project.ToJSON();
        //    project.Clear();

        //    Project project2 = Project.FromJSON(json);
        //}

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
            string panelJson = "{ \"Type\":\"BH.oM.Structural.Panel\",\"Primitive\":\"BH.oM.Structural.Panel; BHoM; Version=1.0.0.0; Culture=neutral; PublicKeyToken=null\",\"Properties\":{ \"Edges\":{ \"Primitive\":\"group\",\"groupType\":\"BH.oM.Geometry.Curve\",\"group\":[{\"Primitive\":\"line\",\"start\":[24.5778681994532,-2.74803939183246,45.315],\"end\":[29.5556354116369,-9.05481358976734,45.315]},{\"Primitive\":\"line\",\"start\":[29.5556354116369,-9.05481358976734,45.315],\"end\":[29.5556354116369,-9.05481358976734,48.065]},{\"Primitive\":\"line\",\"start\":[29.5556354116369,-9.05481358976734,48.065],\"end\":[24.5778681994532,-2.74803939183246,48.065]},{\"Primitive\":\"line\",\"start\":[24.5778681994532,-2.74803939183246,48.065],\"end\":[24.5778681994532,-2.74803939183246,45.315]}]},\"ThicknessProperty\":\"ab645fdd-b100-4d87-ab16-84ab6e053218\",\"BHoM_Guid\":\"ea18e6ab-1cae-4cbd-9f24-05c9f608f486\",\"CustomData\":{\"RevitId\":804997,\"RevitType\":\"Wall\"}}}";
            Panel panel = BHoMObject.FromJSON(panelJson, Project.ActiveProject) as Panel;

            panelJson = "{ \"Type\":\"BH.oM.Structural.Panel\",\"Primitive\":\"BH.oM.Structural.Panel; BHoM; Version=1.0.0.0; Culture=neutral; PublicKeyToken=null\",\"Properties\":{ \"Edges\":{ \"Primitive\":\"group\",\"groupType\":\"BH.oM.Geometry.Curve\",\"group\":[{\"Primitive\":\"line\",\"start\":[28.8385630649439,-7.90417884756682,45.315],\"end\":[31.0639227821543,-6.14776228848485,45.315]},{\"Primitive\":\"line\",\"start\":[31.0639227821543,-6.14776228848485,45.315],\"end\":[31.0639227821543,-6.14776228848485,48.065]},{\"Primitive\":\"line\",\"start\":[31.0639227821543,-6.14776228848485,48.065],\"end\":[28.8385630649439,-7.90417884756682,48.065]},{\"Primitive\":\"line\",\"start\":[28.8385630649439,-7.90417884756682,48.065],\"end\":[28.8385630649439,-7.90417884756682,45.315]}]},\"ThicknessProperty\":\"460c4041-6c21-41cb-9b68-0e34250ee406\",\"BHoM_Guid\":\"6c21eefa-8c84-4256-8979-f232bc3e8917\",\"CustomData\":{\"RevitId\":804999,\"RevitType\":\"Wall\"}}}";
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
