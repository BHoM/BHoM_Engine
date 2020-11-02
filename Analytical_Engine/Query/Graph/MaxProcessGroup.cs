using BH.oM.Analytical.Fragments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Analytical
{
    public static partial class Query
    {
        public static int LargestProcessGroup(this LayoutHelperFragment layoutHelper)
        {
            if (layoutHelper.EntityGroups.Count() == 0)
                return 0;
            return layoutHelper.EntityGroups.Select(c => c.EntityGuids.Count()).Max();
        }
    }
}
