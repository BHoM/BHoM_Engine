using BH.oM.Geometry;
using BH.oM.Structural.Elements;
using BH.Engine.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Structure
{
    public static partial class Create
    {

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Grid CreateGrid(Line line)
        {
            Plane plane = new Plane(line.Start, Geometry.Query.GetCrossProduct(line.End - line.Start, Vector.ZAxis));
            return new Grid(plane, line);
        }

        /***************************************************/

        public static Grid CreateGrid(Point origin, Vector direction)
        {
            Plane plane = new Plane(origin, Geometry.Query.GetCrossProduct(direction, Vector.ZAxis));
            Line line = new Line(origin, origin + direction * 20);
            return new Grid(plane, line);
        }

        /***************************************************/
    }
}
