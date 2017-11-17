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

        public static Ray Ray(Speaker speaker, Receiver receiver)
        {
            return new Ray(new Polyline(new List<Point> { speaker.Location, receiver.Location }), speaker.SpeakerID, receiver.ReceiverID);
        }
    }
}
