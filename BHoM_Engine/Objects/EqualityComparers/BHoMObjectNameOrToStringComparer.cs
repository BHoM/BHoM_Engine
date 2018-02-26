using System;
using System.Collections.Generic;
using BH.oM.Base;

namespace BH.Engine.Base.Objects
{
    public class BHoMObjectNameOrToStringComparer : IEqualityComparer<IBHoMObject>
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public bool Equals(IBHoMObject x, IBHoMObject y)
        {
            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            //Check if the GUIDs are the same
            if (x.BHoM_Guid == y.BHoM_Guid)
                return true;

            string xName = !string.IsNullOrWhiteSpace(x.Name) ? x.Name : x.ToString();
            string yName = !string.IsNullOrWhiteSpace(y.Name) ? y.Name : y.ToString();

            return xName == yName;
        }

        /***************************************************/

        public int GetHashCode(IBHoMObject obj)
        {
            //Check whether the object is null
            if (Object.ReferenceEquals(obj, null)) return 0;

            return (!string.IsNullOrWhiteSpace(obj.Name) ? obj.Name : obj.ToString()).GetHashCode();
        }

        /***************************************************/
    }

}
