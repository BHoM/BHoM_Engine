using BH.oM.Analytical.Elements;
using BH.oM.Base;
using BH.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Analytical
{
    public static partial class Query
    {
        public static Dictionary<Guid, IBHoMObject> FilterEntities(this Graph graph, Type typeFilter)
        {
            List<IBHoMObject> entities = Base.Query.FilterByType(graph.Entities(), typeFilter).Cast<IBHoMObject>().ToList();

            Dictionary<Guid, IBHoMObject> entityDict = new Dictionary<Guid, IBHoMObject>();

            entities.ForEach(ent => entityDict.Add(ent.BHoM_Guid, ent));

            return entityDict;
        }
    }
}
