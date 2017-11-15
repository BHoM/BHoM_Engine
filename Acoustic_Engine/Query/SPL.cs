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

        public static double SPL(this List<Ray> rays)
        {
            double SPL = 0;
            for (int i = 0; i < rays.Count; i++)
            {
                SPL += (10 * Math.Log10(Math.Pow(10, 10 / rays[i].GetLength())));
            }
            return SPL;
        }

        /***************************************************/

        public static double SPL(this Ray ray)
        {
            return 10 * Math.Log10(Math.Pow(10, 10 / ray.GetLength()));
        }

        /***************************************************/

        public static SoundLevel SPL(this Room room, Receiver receiver, Speaker speaker, double revTime, Frequency octave)
        {
            Vector deltaPos = receiver.Geometry - speaker.Geometry;
            double distance = deltaPos.GetLength();
            double roomConstant = room.GetRoomConstant(revTime);
            double revDist = room.ReverbDistance(revTime);

            if (distance < revDist) { return new SoundLevel(); }

            double recieverAngle = deltaPos.GetAngle(speaker.Direction) * (180 / Math.PI);
            double orientationFactor = speaker.GetGainFactor(recieverAngle, octave);
            double gain = speaker.Gains[octave] * Math.Pow(10, orientationFactor / 10);
            double level = (gain / (4.0 * Math.PI * distance * distance)) + (4.0 / roomConstant);

            return new SoundLevel(level, receiver.ReceiverID, speaker.SpeakerID, octave);
        }
    }
}
