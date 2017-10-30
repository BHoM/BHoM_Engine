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

        public static List<SPL> ComputeSPL(this Room room, List<Speaker> speakers, List<double> revTimes, Frequency octave)
        {
            List<SPL> results = new List<SPL>();

            List<Receiver> receivers = room.Samples;

            for (int i = 0; i < receivers.Count; i++)
            {
                for (int j = 0; j < receivers.Count; j++)
                {
                    Vector deltaPos = receivers[i].Geometry - speakers[j].Geometry;
                    double distance = deltaPos.GetLength();
                    double roomConstant = room.GetRoomConstant(revTimes[i]);
                    double revDist = room.ReverbDistance(revTimes[i]);

                    if (distance < revDist)
                    {
                        results.Add(new SPL() { Value = 0, ReceiverID = receivers[i].ReceiverID, Octave = octave });
                        continue;
                    }

                    double recieverAngle = deltaPos.GetAngle(speakers[j].Direction) * (180 / Math.PI);
                    double orientationFactor = speakers[j].GetGainFactor(recieverAngle, octave);
                    double gain = speakers[j].Gains[octave] * Math.Pow(10, orientationFactor / 10);
                    double level = (gain / (4.0 * Math.PI * distance * distance)) + (4.0 / roomConstant);
                    results.Add(new SPL(level, receivers[i].ReceiverID, speakers[i].SpeakerID, octave));
                }
            }
            return results;
        }
    }
}
