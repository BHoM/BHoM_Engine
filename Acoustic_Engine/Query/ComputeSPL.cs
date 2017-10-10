using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Acoustic;

namespace BH.Engine.Acoustic
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public static double SoundLevel(this List<Ray> rays)
        {
            double SPL = 0;
            for (int i = 0; i < rays.Count; i++)
            {
                // Directivity calculation to be added.
                // Adequate to multiple geometrical attenuation due to multiple ray bounces
                SPL += (10 * Math.Log10(Math.Pow(10, 10 / rays[i].GetLength())));         
            }
            return SPL;
        }
    }
}
