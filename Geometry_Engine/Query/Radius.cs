using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static double GetRadius(this Arc arc)
        {
            Point centre = arc.GetCentre();
            if (centre != null)
                return centre.GetDistance(arc.Start);
            else
                return 0;
        }
    }
}
