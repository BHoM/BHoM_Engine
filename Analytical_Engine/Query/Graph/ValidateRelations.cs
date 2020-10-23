using BH.oM.Analytical.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Analytical
{
    public static partial class Query
    {
        public static void ValidateRelations(this Graph graph)
        {
            List<IRelation> relations = graph.Relations.FindAll(ent => ent is IValidator);
        }
    }
}
