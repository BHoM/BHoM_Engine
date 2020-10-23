using BH.oM.Analytical.Elements;
using BH.oM.Base;
using BH.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Dimensional;

namespace BH.Engine.Analytical
{
    public static partial class Query
    {
        public static Dictionary<Guid, IBHoMObject> FilterEntities(this Graph graph, Type typeFilter)
        {
            Dictionary<Guid, IBHoMObject> entityDict = new Dictionary<Guid, IBHoMObject>();
            List<IBHoMObject> filtered = graph.Entities().Where(x => typeFilter.IsAssignableFrom(x.GetType())).ToList();
            filtered.ForEach(obj => entityDict.Add(obj.BHoM_Guid, obj));
            return entityDict;
        }
    }
}
