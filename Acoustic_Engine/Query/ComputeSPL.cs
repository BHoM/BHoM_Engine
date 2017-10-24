using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Acoustic;
using BH.oM.Geometry;
using BH.Engine.Geometry;

namespace BH.Engine.Acoustic
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public static double ComputeSPL(this List<Ray> rays)
        {
            double SPL = 0;
            for (int i = 0; i < rays.Count; i++)
            {
                SPL += (10 * Math.Log10(Math.Pow(10, 10 / rays[i].GetLength())));
            }
            return SPL;
        }

        /***************************************************/

        public static List<SPL> ComputeSPL(this Room room, Speaker speaker, double speech, double revTime, Frequency octave)
        {
            List<SPL> results = new List<SPL>();

            List<Receiver> receivers = room.Samples;
            Dictionary<Frequency, double> gains = new Dictionary<Frequency, double> { { Frequency.Hz500, 1.6 }, { Frequency.Hz2000, 5.3 } };
            speaker = new Speaker(speaker.Geometry, speaker.Direction, speaker.Category, speaker.SpeakerID, gains);
            double roomConstant = room.GetRoomConstant(revTime);

            for (int i = 0; i < receivers.Count; i++)
            {
                Vector deltaPos = receivers[i].Geometry - speaker.Geometry;
                double distance = deltaPos.GetLength();
                double revDist = room.GetReverbDistance(revTime);

                if (distance < revDist)
                {
                    results.Add(new SPL() { Value = 0, ReceiverID = receivers[i].ReceiverID, Octave = octave });
                    continue;
                }

                double recieverAngle = deltaPos.GetAngle(speaker.Direction) * (180 / Math.PI);
                double orientationFactor = speaker.GetGainFactor(recieverAngle, octave);
                double gain = speaker.Gains[octave] * Math.Pow(10, orientationFactor / 10);
                double level = (gain / (4.0 * Math.PI * distance * distance)) + (4.0 / roomConstant);
                results.Add(new SPL(level, receivers[i].ReceiverID, octave));
            }
            return results;
        }
    }
}
