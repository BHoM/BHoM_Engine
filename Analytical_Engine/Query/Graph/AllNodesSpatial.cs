using BH.Engine.Base;
using BH.oM.Analytical.Elements;
using BH.oM.Base;
using BH.oM.Dimensional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Analytical
{
    public static partial class Query
    {
        public static bool SpatialEntities(this Graph graph)
        {
            foreach(KeyValuePair<Guid, IBHoMObject>kvp in graph.Entities)
            {
                if (!(kvp.Value is IElement0D))
                    return false;
            }    
            return true;
        }
    }
}
