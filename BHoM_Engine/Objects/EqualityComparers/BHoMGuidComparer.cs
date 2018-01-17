using System;
using System.Collections.Generic;
using BH.oM.Base;

namespace BH.Engine.Base.Objects
{
    public class BHoMGuidComparer : IEqualityComparer<IObject>
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public bool Equals(IObject x, IObject y)
        {
            //Check if the GUIDs are the same
            return (x.BHoM_Guid == y.BHoM_Guid);
        }

        /***************************************************/

        public int GetHashCode(IObject obj)
        {
            //Check whether the object is null
            if (Object.ReferenceEquals(obj, null)) return 0;

            return obj.BHoM_Guid.GetHashCode();
        }

        /***************************************************/
    }
}

