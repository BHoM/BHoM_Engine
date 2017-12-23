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

        public static Rasti Rasti(double value, int receiverID)
        {
            return new Rasti()
            {
                Value = value,
                ReceiverID = receiverID,
            };
        }

        /***************************************************/
    }
}
