using BH.oM.Geometry;
using BH.oM.Architecture.Elements;
using System.Collections.Generic;

namespace BH.Engine.Architecture.Elements
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Grid Grid(ICurve curve)
        {
            return new Grid
            {
                Curve = Geometry.Modify.IProject(curve, Plane.XY)
            };
        }

        /***************************************************/

        public static Grid Grid(Point origin, Vector direction)
        {
            Line line = new Line { Start = new Point { X = origin.X, Y = origin.Y, Z = 0 }, End = origin + direction * 20 };
            return new Grid { Curve = line };
        }

        /***************************************************/
    }
}
