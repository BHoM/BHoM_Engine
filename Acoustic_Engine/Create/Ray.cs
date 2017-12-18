using BH.oM.Acoustic;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Acoustic
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public static Ray Ray(Polyline path, int source, int target, List<int> bouncingPattern = null)
        {
            return new Ray()
            {
                Geometry = path,
                SpeakerID = source,
                ReceiverID = target,
                PanelsID = bouncingPattern
            };
        }

        /***************************************************/

        public static Ray Ray(Speaker speaker, Receiver receiver)
        {
            return Create.Ray(new Polyline(new List<Point> { speaker.Location, receiver.Location }), speaker.SpeakerID, receiver.ReceiverID);
        }
    }
}
