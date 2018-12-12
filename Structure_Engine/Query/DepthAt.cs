using BH.oM.Geometry;
using BH.oM.Structure.Properties.Section.ShapeProfiles;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [NotImplemented]
        public static double DepthAt(this IProfile section, double x)
        {
            IntegrationSlice slice = Engine.Geometry.Query.SliceAt(section.Edges, x, 1, oM.Geometry.Plane.YZ);
            //Slice slice = SliceAt(x, 1, Plane.YZ());// new Plane(Point.Origin, Vector.XAxis()));
            return slice.Length;
        }

        /***************************************************/

        [NotImplemented]
        public static double DepthAt(this IProfile section, double x, ref double[] range)
        {
            IntegrationSlice slice = Engine.Geometry.Query.SliceAt(section.Edges, x, 1, oM.Geometry.Plane.YZ);
            //Slice slice = SliceAt(x, 1, Plane.YZ());
            range = slice.Placement;
            return slice.Length;
        }

        /***************************************************/
    }
}
