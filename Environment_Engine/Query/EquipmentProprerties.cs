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
        public static IEquipmentProperties EquipmentProperties(this IEquipment Equipment)
        {
            if (Equipment is Emitter)
                return (Equipment as Emitter).EmitterProperties;
            if(Equipment is AirHandlingUnit)
                return (Equipment as AirHandlingUnit).AirHandlingUnitProperties;

            return null;
        }
    }
}
