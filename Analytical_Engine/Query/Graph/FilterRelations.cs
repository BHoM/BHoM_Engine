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
        public static List<IRelation> FilterRelations(this Graph graph, Type typeFilter)
        {
            return Base.Query.FilterByType(graph.Relations, typeFilter).Cast<IRelation>().ToList();

        }
    }
}
