using BH.oM.Geometry;
using System;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods - Vectors                  ****/
        /***************************************************/

        public static Vector Reverse(this Vector vector)
        {
            return new Vector { X = -vector.X, Y = -vector.Y, Z = -vector.Z };
        }


        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static Line Reverse(this Line line)
        {
            return new Line { Start = line.End, End = line.Start, Infinite = line.Infinite };
        }
    }
}