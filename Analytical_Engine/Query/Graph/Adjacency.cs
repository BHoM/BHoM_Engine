using BH.Engine.Base;
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
        public static Dictionary<Guid, List<Guid>> Adjacency(this Graph graph, RelationDirection relationDirection = RelationDirection.Forwards)
        {
            //should add input to control directionality
            Dictionary<Guid, List<Guid>> adjacency = new Dictionary<Guid, List<Guid>>();
            graph.Entities.ToList().ForEach(n => adjacency.Add(n.Key, new List<Guid>()));
            foreach(Guid vertex in graph.Entities.Keys.ToList())
            {
                adjacency[vertex].AddRange(graph.IncomingNodes(vertex));
                foreach (Guid d in graph.Destinations(vertex))
                {
                    if (!adjacency[vertex].Contains(d))
                        adjacency[vertex].Add(d);
                }

            }
            return adjacency;
        }
    }
}
