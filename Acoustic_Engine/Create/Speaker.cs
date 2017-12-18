using BH.oM.Acoustic;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BH.Engine.Acoustic
{
    public static partial class Create
    {
        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public static Speaker Speaker(Point location, Vector direction = null, double emissiveLevel = 100, string category = "Omni")
        {
            return new Speaker()
            {
                Location = location,
                Direction = direction,
                EmissiveLevel = emissiveLevel,
                Category = category,
                Gains = new Dictionary<Frequency, double>() { { Frequency.Hz500, 1.6 }, { Frequency.Hz2000, 5.3 } }
            };
        }

        /***************************************************/

        public static Speaker Speaker(Point location, Vector direction, string category, Dictionary<Frequency, double> gains = null, Dictionary<Frequency, double[,]> directivity = null)
        {
            return new Speaker()
            {
                Location = location,
                Direction = direction,
                Category = category,
                Gains = gains,
                Directivity = directivity,
            };
        }
    }
}
