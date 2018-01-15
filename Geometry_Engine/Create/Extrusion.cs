using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Extrusion Extrusion(ICurve curve, Vector direction, bool capped = true)
        {
            return new Extrusion
            {
                Curve = curve,
                Direction = direction,
                Capped = capped
            }; 
        }

        /***************************************************/
    }
}
