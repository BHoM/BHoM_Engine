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

        public static SoundLevel SoundLevel(this Ray ray, Frequency frequency = Frequency.Sum)
        {
            List<Line> raySegments = ray.Geometry.GetExploded();
            double spl = 0;
            for (int i=0; i< raySegments.Count; i++)
                spl += 10 * Math.Log10(Math.Pow(10, 10 / raySegments[i].GetLength()));
            return Create.SoundLevel(spl, ray.ReceiverID, ray.SpeakerID, frequency);
        }

        /***************************************************/
    }
}
