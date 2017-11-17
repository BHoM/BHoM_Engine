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

        public static double SoundLevelsSum(this List<double> spl)
        {
            double SPL = 0;
            for (int i = 0; i < spl.Count; i++)
                SPL += (10 * Math.Log10(Math.Pow(10, spl[i] / 10)));
            return SPL;
        }
        
        /***************************************************/

        public static SoundLevel SoundLevelsSum(this List<SoundLevel> spl)
        {
            SoundLevel totalLevel = new SoundLevel();
            for (int i = 0; i < spl.Count; i++)
                totalLevel += spl[i];
            return totalLevel;
        }
    }
}
