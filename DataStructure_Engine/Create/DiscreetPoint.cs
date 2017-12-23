using BH.oM.DataStructure;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.DataStructure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static DiscreetPoint DiscreetPoint(int x, int y, int z)
        {
            return new DiscreetPoint { X = x, Y = y, Z = z };
        }

        /***************************************************/

        public static DiscreetPoint DiscreetPoint(Point point, double step = 1.0)
        {
            return new DiscreetPoint {
                X = (int)Math.Floor(point.X / step),
                Y = (int)Math.Floor(point.Y / step),
                Z = (int)Math.Floor(point.Z / step)
            };
        }

        /***************************************************/
    }
}
