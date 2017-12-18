using BH.oM.Acoustic;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BH.Engine.Acoustic
{
    public static partial class Create
    {
        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public static Receiver Receiver(Point location)
        {
            return new Receiver()
            {
                Location = location,
            };
        }

        /***************************************************/

        public static Receiver Receiver(Point location, string category)
        {
            return new Receiver()
            {
                Location = location,
                Category = category,
            };
        }
    }
}
