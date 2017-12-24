using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Face Face(int a, int b, int c, int d = -1)
        {
            return new Face
            {
                A = a,
                B = b,
                C = c,
                D = d
            };
        }

        /***************************************************/
    }
}
