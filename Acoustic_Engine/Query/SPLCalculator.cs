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
        /// <summary>
        /// Incoherent sound level signal addition
        /// </summary>
        /// <param name="rays"></param>
        /// <returns></returns>
        public static double  Solve(List<Ray> rays)
        {
            double SPL = 0;
            for (int i=0; i<rays.Count; i++)
            {
                SPL += (10 * Math.Log10 (  Math.Pow(10, 10/rays[i].GetLength()  ) ) );         // Directivity calculation to be added.
            }
            return SPL;
        }

        public static double Solve(List<double> spl)
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
