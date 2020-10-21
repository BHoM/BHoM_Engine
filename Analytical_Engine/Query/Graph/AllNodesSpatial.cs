using BH.Engine.Base;
using BH.oM.Analytical.Elements;
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Analytical
{
    public static partial class Query
    {
        public static bool AllNodesSpatial(this Graph graph)
        {
            foreach(KeyValuePair<Guid, IBHoMObject>kvp in graph.Entities)
            {
                if (!(kvp.Value is INode))
                    return false;
            }    
            return true;
        }
    }
}
