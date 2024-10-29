using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace BH.Engine.Verification
{
    public static partial class Query
    {
        public static IComparable IIdentifier(this object obj)
        {
            object result;
            if (!BH.Engine.Base.Compute.TryRunExtensionMethod(obj, nameof(Identifier), out result))
            {
                //error
                return null;
            }

            return (IComparable)result;
        }

        public static IComparable Identifier(this IBHoMObject obj)
        {
            return obj.BHoM_Guid;
        }
    }
}
