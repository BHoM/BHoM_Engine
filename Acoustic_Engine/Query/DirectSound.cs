using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Threading.Tasks;
using Alea;
using Alea.Parallel;
using OpenCL.Net;
using Cudafy;
using Cudafy.Host;
using Cudafy.Translator;
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

        public static Ray DirectSound(this Receiver target, Speaker source, List<Panel> surfaces)
        {
            Polyline path = Create.Polyline(source.Geometry, target.Geometry);
            Ray ray = new Ray(path, source.SpeakerID, target.ReceiverID);
            return Verify.IsObstructed(ray, surfaces) ? ray : default(Ray);
        }

        /***************************************************/

        public static List<Ray> DirectSound(this List<Receiver> targets, List<Speaker> sources, List<Panel> surfaces)
        {
            List<Ray> rays = new List<Ray>();
            for (int i = 0; i < sources.Count; i++)
                for (int j = 0; j < targets.Count; j++)
                    rays.Add(DirectSound(targets[j], sources[i], surfaces));
            return rays;
        }

        /***************************************************/

        public static List<Ray> DirectSound(this Room room, List<Speaker> speakers, List<double> revTimes, Frequency frequency)
        {
            List<Ray> directRays = new List<Ray>();
            List<Receiver> receivers = room.Samples;
            for (int i = 0; i < receivers.Count; i++)
            {
                foreach (Speaker speaker in speakers)
                {
                    Vector deltaPos = receivers[i].Geometry - speaker.Geometry;
                    double distance = deltaPos.GetLength();
                    double roomConstant = room.RoomConstant(revTimes.ElementAt(i));
                    double revDist = room.ReverbDistance(revTimes.ElementAt(i));

                    double recieverAngle = deltaPos.GetAngle(speaker.Direction) * (180 / Math.PI);
                    double orientationFactor = speaker.GainFactor(recieverAngle, frequency);
                    double gain = speaker.Gains[frequency] * Math.Pow(10, orientationFactor / 10);
                    double level = (gain / (4.0 * Math.PI * distance * distance)) + (4.0 / roomConstant);
                    double amb_pascal = level;
                    directRays.Add(new Ray(Create.Polyline(receivers[i].Geometry, speaker.Geometry), speaker.SpeakerID, receivers[i].ReceiverID));
                }
            }
            return directRays;
        }

        /***************************************************/

        public static SoundLevel DirectSound(this Receiver receiver, List<Speaker> speakers, Room room, double revTime, Frequency frequency)
        {
            SoundLevel directSound = new SoundLevel();
            foreach (Speaker speaker in speakers)
            {
                Vector deltaPos = receiver.Geometry - speaker.Geometry;
                double distance = deltaPos.GetLength();
                double roomConstant = room.RoomConstant(revTime);
                double revDist = room.ReverbDistance(revTime);

                double recieverAngle = deltaPos.GetAngle(speaker.Direction) * (180 / Math.PI);
                double orientationFactor = speaker.GainFactor(recieverAngle, frequency);
                double gain = speaker.Gains[frequency] * Math.Pow(10, orientationFactor / 10);
                directSound += new SoundLevel((gain / (4.0 * Math.PI * distance * distance)) + (4.0 / roomConstant), receiver.ReceiverID, -1, frequency);
            }
            return directSound;
        }
    }
}
