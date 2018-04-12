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
            if (!arc.Start.IsValid() || !arc.End.IsValid() || !arc.Middle.IsValid())
                return false;
            double sqTol = tolerance * tolerance;
            if (arc.Start.SquareDistance(arc.End) < sqTol || arc.Start.SquareDistance(arc.Middle) < sqTol || arc.Middle.SquareDistance(arc.End) < sqTol)
                return false;

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
