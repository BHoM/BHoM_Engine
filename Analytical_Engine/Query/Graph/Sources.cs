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
        public static List<Guid> Sources(this Graph graph)
        {
            //entity is a source if it never appears as target
            List<Guid> targets = graph.Relations.Select(x => x.Target).Distinct().ToList();
            List<Guid> sources = graph.Entities.Keys.ToList().Except(targets).ToList();
            return sources;
        }
    }
}
