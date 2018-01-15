using BH.oM.Geometry;
using BH.oM.Structural.Properties;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static double WidthAt(this IGeometricalSection section, double y)
        {
            IntegrationSlice slice = Engine.Geometry.Query.SliceAt(section.Edges, y, 1, oM.Geometry.Plane.XZ);
            //Slice slice = SliceAt(y, 1, Plane.XZ());// new Plane(Point.Origin, Vector.YAxis()));
            return slice.Length;
        }

        /***************************************************/

        public static double WidthAt(this IGeometricalSection section, double y, ref double[] range)
        {
            IntegrationSlice slice = Engine.Geometry.Query.SliceAt(section.Edges, y, 1, oM.Geometry.Plane.XZ);
            //Slice slice = SliceAt(y, 1, Plane.XZ());
            range = slice.Placement;
            return slice.Length;
        }

        /***************************************************/
    }
}
