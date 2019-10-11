using BH.oM.Base;
using BH.Engine.Base;
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
            return obj.FindFragment<HashFragment>(); 
        }
    }
}

