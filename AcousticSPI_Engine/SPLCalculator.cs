using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BHoM.Acoustic;

namespace AcousticSPI_Engine
{
    public static class SPLCalculator
    {
        
        public static List<double>  Solve(List<Ray> rays)
        {
            List<double> SPL = new List<double>();
            for (int i=0; i<rays.Count; i++)
            {
                SPL.Add(20 * Math.Log10(rays[i].Length()));         // Directivity calculation to be added.
            }
            return SPL;
        }

       /*public static List<double> Sum(List<double> spl)
        {
            List<double> SPL = new List<double>();
                return SPL;
        }*/
    }
}
