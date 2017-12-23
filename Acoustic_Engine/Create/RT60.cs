using BH.oM.Acoustic;
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
        /**** Public Methods                            ****/
        /***************************************************/

        public static RT60 RT60(double value, int receiverID, int speakerID)
        {
            return new RT60()
            {
                Value = value,
                ReceiverID = receiverID,
                SpeakerID = speakerID
            };
        }

        /***************************************************/
    }
}
