using BH.oM.Base;
using System;

namespace BH.Engine.Verification
{
    public static partial class Query
    {
        /***************************************************/
        /****             Interface Methods             ****/
        /***************************************************/

        public static IComparable IIdentifier(this object obj)
        {
            if (obj == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not query identifier from a null object.");
                return null;
            }

            object result;
            if (!BH.Engine.Base.Compute.TryRunExtensionMethod(obj, nameof(Identifier), out result))
            {
                BH.Engine.Base.Compute.RecordError($"Identifier query failed because object of type {obj.GetType().Name} is currently not supported.");
                return null;
            }

            return (IComparable)result;
        }


        /***************************************************/
        /****              Public Methods               ****/
        /***************************************************/

        public static IComparable Identifier(this IBHoMObject obj)
        {
            if (obj == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not query identifier from a null object.");
                return null;
            }

            return obj.BHoM_Guid;
        }

        /***************************************************/
    }
}
