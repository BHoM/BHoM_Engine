using System;
using System.Collections.Generic;
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

        public static SoundLevel SoundLevel(this Ray ray, Frequency frequency = Frequency.Sum)
        {
            double spl = 0;
            foreach (Line raySegment in ray.Path.SubParts())
                spl += 10 * Math.Log10(Math.Pow(10, 10 / raySegment.Length()));
            return Create.SoundLevel(spl, ray.ReceiverID, ray.SpeakerID, frequency);
        }

        /***************************************************/
    }
}
