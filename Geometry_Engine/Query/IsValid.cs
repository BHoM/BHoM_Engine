using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Vectors                  ****/
        /***************************************************/

        public static bool IsValid(this Point point)
        {
            return !(double.IsNaN(point.X) || double.IsNaN(point.Y) || double.IsNaN(point.Z));
        }

        /***************************************************/

        public static bool IsValid(this Vector v)
        {
            return !(double.IsNaN(v.X) || double.IsNaN(v.Y) || double.IsNaN(v.Z));
        }

        
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool IsValid(IGeometry geometry)
        {
            return true;
        }

        /***************************************************/

        public static bool IsValid(this Arc arc, double tolerance = Tolerance.Distance)
        {
            //TODO: Returning true for all for now until method is expanded to all objects
            return true;
        }


        /***************************************************/
        /**** Public Methods - Interface                ****/
        /***************************************************/

        public static bool IIsValid(IGeometry geometry)
        {
            return IsValid(geometry as dynamic);
        }

        /***************************************************/
    }
}
