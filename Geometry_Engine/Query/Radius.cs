using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static double Radius(this Arc arc)
        {
            Point centre = arc.Centre();
            if (centre != null)
                return centre.GetDistance(arc.Start);
            else
                return 0;
        }

        /***************************************************/
    }
}
