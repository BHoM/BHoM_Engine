using BH.oM.Geometry;
using BH.oM.Structural.Elements;
using BH.oM.Structural.Properties;
using BH.Engine.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Structure
{
    public static partial class Query
    {

        //***************************************************/
        //**** Public Methods                            ****/
        //***************************************************/

        public static double GetWidthAt(this IGeometricalSection section, double y)
        {
            IntegrationSlice slice = Geometry.Query.GetSliceAt(section.Edges, y, 1, Plane.XZ);
            //Slice slice = GetSliceAt(y, 1, Plane.XZ());// new Plane(Point.Origin, Vector.YAxis()));
            return slice.Length;
        }

        /***************************************************/

        public static double WidthAt(this IGeometricalSection section, double y, ref double[] range)
        {
            IntegrationSlice slice = Geometry.Query.GetSliceAt(section.Edges, y, 1, Plane.XZ);
            //Slice slice = GetSliceAt(y, 1, Plane.XZ());
            range = slice.Placement;
            return slice.Length;
        }

        /***************************************************/
        public static double DepthAt(this IGeometricalSection section, double x)
        {
            IntegrationSlice slice = Geometry.Query.GetSliceAt(section.Edges, x, 1, Plane.YZ);
            //Slice slice = GetSliceAt(x, 1, Plane.YZ());// new Plane(Point.Origin, Vector.XAxis()));
            return slice.Length;
        }

        /***************************************************/
        public static double DepthAt(this IGeometricalSection section, double x, ref double[] range)
        {
            IntegrationSlice slice = Geometry.Query.GetSliceAt(section.Edges, x, 1, Plane.YZ);
            //Slice slice = GetSliceAt(x, 1, Plane.YZ());
            range = slice.Placement;
            return slice.Length;
        }

        /***************************************************/

    }
}
