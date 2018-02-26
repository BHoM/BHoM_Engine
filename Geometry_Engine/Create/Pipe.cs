using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Pipe Pipe(ICurve centreline, double radius, bool capped = true)
        {
            return new Pipe
            {
                Centreline = centreline,
                Radius = radius,
                Capped = capped
            };
        }

        /***************************************************/
    }
}
