using BH.oM.Geometry;
using BH.oM.Architecture.Elements;
using System.Collections.Generic;
using BH.Engine.Geometry;

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

        public static Grid Grid(Point origin, Vector direction, double length = 20)
        {
            Line line = new Line { Start = new Point { X = origin.X, Y = origin.Y, Z = 0 }, End = origin + new Vector { X = direction.X, Y = direction.Y, Z = 0 }.Normalise() * length };
            return new Grid { Curve = line };
        }

        /***************************************************/
    }
}