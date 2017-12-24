using BH.oM.Geometry;
using BH.oM.Structural.Elements;

namespace BH.Engine.Structure
{
    public static partial class Query
    {

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Plane GetPlane(this Storey storey)
        {
            return new Plane(new Point(0, 0, storey.Elevation), new Vector(0, 0, 1));
        }

        /***************************************************/


    }
}
