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
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Bar Bar(Line line)
        {
            Bar bar = new Bar();
            bar.StartNode = new Node(line.Start);
            bar.EndNode = new Node(line.End);
            bar.SetGeometry(line);
            return bar;
        }

        /***************************************************/

        public static Bar Bar(Point pointA, Point pointB)
        {
            Bar bar = new Bar();
            bar.StartNode = new Node(pointA);
            bar.EndNode = new Node(pointB);
            bar.SetGeometry(new Line(pointA, pointB));
            return bar;
        }
    }
}
