using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Vectors                  ****/
        /***************************************************/

        public static bool IsValid(Point point)
        {
            return !(double.IsNaN(point.X) || double.IsNaN(point.Y) || double.IsNaN(point.Z));
        }

        /***************************************************/

        public static bool IsValid(Vector v)
        {
            return !(double.IsNaN(v.X) || double.IsNaN(v.Y) || double.IsNaN(v.Z));
        }



        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool IsValid(IBHoMGeometry geometry)
        {
            return true;
        }

        /***************************************************/

        public static bool IsValid(this Arc arc)
        {
            if (!arc.Start.IsValid() || !arc.End.IsValid() || !arc.Middle.IsValid())
                return false;

            if (arc.Start.GetSquareDistance(arc.End) < Tolerance.SqrtDist || arc.Start.GetSquareDistance(arc.Middle) < Tolerance.SqrtDist || arc.Middle.GetSquareDistance(arc.End) < Tolerance.SqrtDist)
                return false;

            return true;
        }


        /***************************************************/
        /**** Public Methods - Interface                ****/
        /***************************************************/

        public static bool IIsValid(IBHoMGeometry geometry)
        {
            return IsValid(geometry as dynamic);
        }
    }
}
