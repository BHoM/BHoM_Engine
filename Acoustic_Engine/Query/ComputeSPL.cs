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

        private static double ComputeSPL<Octave>(double speech, double revTime, Receiver receiver, Speaker inputSpeaker, Room zone, double frequency, double octave, double closestDist,
                                          out double level, out double amb_pascals, out double revDist, out double timeConstant, out double closestdist, out double gain)
            where Octave : Octaves
        {
            if (!typeof(Octave).IsAssignableFrom(typeof(Octaves)))
            {
                throw new ArgumentException("Generic Type Octave must be assignable from BH.oM.Acoustic.Frequency Enumerated Type");
            }

            Speaker speaker = new Speaker(inputSpeaker.Geometry, inputSpeaker.Direction, inputSpeaker.Category, inputSpeaker.SpeakerID,
                                          new Dictionary<Octaves, double> { { Octaves.Hz500, 1.6 }, { Octaves.Hz2000, 5.3 } });

            Vector deltaPos = receiver.Geometry - speaker.Geometry;
            double recieverAngle = Engine.Geometry.Query.GetAngle(deltaPos, speaker.Direction) * (180 / Math.PI);
            double distance = deltaPos.GetLength();
            double orientationFactor = speaker.GetGainAngleFactor(recieverAngle, octave);

            gain = speaker.GetGain(frequency, octave) * Math.Pow(10, orientationFactor / 10);

            double volume = zone.Volume;
            double sAlpha = (0.163 * volume / revTime) - (4.0 * 2.6 * volume / 1000);  // It would be good to clarify all those constants
            double alpha = sAlpha / zone.Area;

            double roomConstant = sAlpha / (1 - alpha);
            if (revTime < 0.01)
                roomConstant = Double.PositiveInfinity;

            revDist = Math.Sqrt(0.0032 * volume / revTime);
            timeConstant = revTime / 13.8155;    // Only used outside of the loop ... Clearly something wrong here

            level = (gain / (4.0 * Math.PI * distance * distance)) + (4.0 / roomConstant);

            if (distance < closestDist)
            {
                //highest_level = speech + 10 * Math.Log10((gain / (4 * Math.PI * distance * distance)) + (4 / roomConstant));
                closestdist = distance;
            }

            else
            {
                closestdist = closestDist;
            }

            if (distance > revDist)
            {
                amb_pascals = gain / ((4.0 * Math.PI * distance * distance) + (4.0 / roomConstant));
            }

            else
            {
                amb_pascals = 0.0;
            }
            return level;
        }
    }
}
