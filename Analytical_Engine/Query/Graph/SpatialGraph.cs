using BH.oM.Analytical.Elements;
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
        public static Graph SpatialGraph(this Graph graph)
        {
            Graph spatialGraph = new Graph();
            if(INode.g)
            spatialGraph.Entities = graph.FilterEntities(typeof(IElement0D));
            spatialGraph.Relations = graph.FilterRelations(typeof(IElement1D));
            return spatialGraph;
        }
    }
}
