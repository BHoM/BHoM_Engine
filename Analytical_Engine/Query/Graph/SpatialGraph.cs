using BH.Engine.Spatial;
using BH.oM.Analytical.Elements;
using BH.oM.Dimensional;
using BH.oM.Geometry;
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
            spatialGraph.Entities = graph.FilterEntities(typeof(IElement0D));
            spatialGraph.Relations = graph.FilterRelations(typeof(SpatialRelation));
            spatialGraph.CreateMissingCurves();
            return spatialGraph;
        }

        private static void CreateMissingCurves(this Graph graph)
        {
            foreach(SpatialRelation spatialRelation in graph.Relations)
            {
                if(spatialRelation.Curve == null)
                {
                    IElement0D source = graph.Entities[spatialRelation.Source] as IElement0D;
                    IElement0D target = graph.Entities[spatialRelation.Target] as IElement0D;
                    spatialRelation.Curve = new Line() { Start = source.IGeometry(), End = target.IGeometry() };
                }
            }
        }
    }
}
