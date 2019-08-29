using BH.oM.Base;
using BH.oM.Diffing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Diffing
{
    public static partial class Query
    {
        public static HashFragment GetHashFragment(this IBHoMObject obj)
        {
            int numOfHashFragments = 0;

            if (obj.Fragments.Exists(fragm => fragm?.GetType() == typeof(HashFragment)))
                numOfHashFragments = obj.Fragments.OfType<HashFragment>().Count();

            if (numOfHashFragments == 0)
                return null;

            if (numOfHashFragments > 1)
            {
                BH.Engine.Reflection.Compute.RecordError("BHoM objects may have only one Hash fragment.");
                return null;
            }

            return obj.Fragments.OfType<HashFragment>().First();
        }

    }
}

