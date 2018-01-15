using BH.oM.Geometry;
using BH.oM.Structural.Elements;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Plane Plane(this Storey storey)
        {
            return new Plane { Origin = new Point { X = 0, Y = 0, Z = storey.Elevation }, Normal = new Vector { X = 0, Y = 0, Z = 1 } };
        }

        /***************************************************/
    }
}
