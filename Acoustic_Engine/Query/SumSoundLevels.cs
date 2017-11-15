using BH.oM.Acoustic;
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

        public static double SumSoundLevels(this List<double> spl)
        {
            double SPL = 0;
            for (int i = 0; i < spl.Count; i++)
            {
                SPL += (10 * Math.Log10(Math.Pow(10, spl[i] / 10)));
            }
            return SPL;
        }

        public static SPL SumSoundLevels(this SPL soundA, SPL soundB)
        {
            return new SPL((10 * Math.Log10(Math.Pow(10, soundA.Value / 10))) + (10 * Math.Log10(Math.Pow(10, soundB.Value / 10))),
                            soundA.ReceiverID,
                            -1, // -1 Represents a sourceID = Sum of sources
                            soundA.Frequency);
        }
    }
}
