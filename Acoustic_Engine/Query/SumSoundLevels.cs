using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Acoustic
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public static double SumSoundLevels(List<double> spl)
        {
            double SPL = 0;
            for (int i = 0; i < spl.Count; i++)
            {
                SPL += (10 * Math.Log10(Math.Pow(10, 10 / spl[i])));
            }
            return SPL;
        }
    }
}
