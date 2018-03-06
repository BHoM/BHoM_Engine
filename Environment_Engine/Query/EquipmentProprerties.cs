using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environmental.Interface;
using BH.oM.Environmental.Elements;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static IEquipmentProperties EquipmentProperties(this IEquipment equipment)
        {
            if (equipment is Emitter)
                return (equipment as Emitter).EmitterProperties;
            if(equipment is AirHandlingUnit)
                return (equipment as AirHandlingUnit).AirHandlingUnitProperties;

            return null;
        }

        /***************************************************/
    }
}
