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

        public static Rasti Rasti(SNRatio apparentSnRatio)
        {
            return new Rasti((apparentSnRatio.Value / 2) + 15.0 / 30.0 , apparentSnRatio.ReceiverID);
        }

        /***************************************************/

        public static List<Rasti> Rasti(this Room room, List<Speaker> speakers, List<double> revTimes, double envNoise)
        {
            List<Rasti> rasti = new List<Rasti>();
            List<SNRatio> apparentSnRatio = SoundNoise(room, speakers, revTimes, envNoise);
            for (int i = 0; i< apparentSnRatio.Count; i++)
                rasti.Add(new Rasti((apparentSnRatio[i].Value / 2) + 15.0 / 30.0, apparentSnRatio[i] .ReceiverID));
            return rasti;
        }
    }
}