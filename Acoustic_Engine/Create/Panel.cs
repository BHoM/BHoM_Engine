using BH.oM.Acoustic;
using BH.oM.Geometry;
using System.Collections.Generic;

namespace BH.Engine.Acoustic
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Panel Panel(Mesh surface)
        {
            return new Panel()
            {
                Surface = surface
            };
        }

        /***************************************************/

        public static Panel Panel(Mesh surface, Dictionary<Frequency, double> r)
        {
            return new Panel()
            {
                Surface = surface,
                R = r
            };
        }

        /***************************************************/
    }
}
