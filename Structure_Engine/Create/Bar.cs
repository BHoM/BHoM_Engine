using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structural;
using BH.oM.Structural.Elements;
using BH.oM.Geometry;
using BH.oM.Base;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        public static Bar IBar(object elementA, object elementB )
        {
            if (elementA == null || elementB == null) { return null; }
            return Create.Bar(elementA as dynamic, elementB as dynamic);
        }
        public static Bar IBar(ICurve curve)
        {
            if (curve == null) { return null; }
            return Create.Bar(curve as dynamic);
        }

        public static Bar Bar(Line line)
        {
            Bar bar = new Bar();
            bar.StartNode = new Node(line.Start);
            bar.EndNode = new Node(line.End);
            bar.SetGeometry(line);
            return bar;
        }
        public static Bar Bar(Polyline polyline)
        {
            Bar bar = new Bar();
            bar.StartNode = new Node(polyline.ControlPoints[0]);
            bar.EndNode = new Node(polyline.ControlPoints[1]);
            bar.SetGeometry(new Line(polyline.ControlPoints[0], polyline.ControlPoints[1]));
            return bar;
        }
        public static Bar Bar(NurbCurve curve)
        {
            Bar bar = new Bar();
            bar.StartNode = new Node(curve.ControlPoints[0]);
            bar.EndNode = new Node(curve.ControlPoints[1]);
            bar.SetGeometry(new Line(curve.ControlPoints[0], curve.ControlPoints[1]));
            return bar;
        }

        public static Bar Bar(Point pointA, Point pointB)
        {
            Bar bar = new Bar();
            bar.StartNode = new Node(pointA);
            bar.EndNode = new Node(pointB);
            bar.SetGeometry(new Line(pointA, pointB));
            return bar;
        }
        public static Bar Bar(Node nodeA, Node nodeB)
        {
            Bar bar = new Bar();
            bar.StartNode = nodeA;
            bar.EndNode = nodeB;
            bar.SetGeometry(new Line(nodeA.Point, nodeB.Point));
            return bar;
        }
    }
}
