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

        public static SoundLevel DirectSound(this Receiver receiver, List<Speaker> speakers, Room room, double revTime, Frequency frequency)
        {
            SoundLevel directSound = new SoundLevel();
            foreach (Speaker speaker in speakers)
            {
                Vector deltaPos = receiver.Location - speaker.Location;
                double distance = deltaPos.GetLength();
                double roomConstant = room.RoomConstant(revTime);

                double recieverAngle = deltaPos.GetAngle(speaker.Direction) * (180 / Math.PI);
                double orientationFactor = speaker.GainFactor(recieverAngle, frequency);
                double gain = speaker.Gains[frequency] * Math.Pow(10, orientationFactor / 10);
                directSound += new SoundLevel((speaker.EmissiveLevel / (4.0 * Math.PI * distance * distance)) + (4.0 / roomConstant),
                                               receiver.ReceiverID, -1, frequency);
            }
            return new SoundLevel(directSound.Value + speakers.First().EmissiveLevel, receiver.ReceiverID, -1, frequency);
        }
    }
}
