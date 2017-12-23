using BH.oM.Acoustic;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Acoustic
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Panel Panel(Mesh geometry)
        {
            return new Panel()
            {
                Geometry = geometry
            };
        }

        /***************************************************/

        public static Panel Panel(Mesh geometry, Dictionary<Frequency, double> r)
        {
            return new Panel()
            {
                Geometry = geometry,
                R = r
            };
        }

        /***************************************************/
    }
}
