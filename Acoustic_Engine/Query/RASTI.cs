using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Geometry;
using BH.oM.Acoustic;
using BH.Engine.Geometry;

namespace BH.Engine.Acoustic
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public static Rasti Rasti(this SNRatio apparentSnRatio)
        {
            return new Rasti((apparentSnRatio.Value / 2) + 15.0 / 30.0 , apparentSnRatio.ReceiverID);
        }

        /***************************************************/

        public static List<Rasti> Rasti(this Room room, List<Speaker> speakers, List<double> revTimes, double envNoise)
        {
            List<Frequency> frequencies = new List<Frequency>() { Frequency.Hz500, Frequency.Hz2000 };
            List<SNRatio> apparentSnRatio = SNRatio(room, speakers, revTimes, envNoise, frequencies);
            return apparentSnRatio.Select(aSNR => new Rasti((aSNR.Value / 2) + 15.0 / 30.0, aSNR.ReceiverID)).ToList();
        }
    }
}