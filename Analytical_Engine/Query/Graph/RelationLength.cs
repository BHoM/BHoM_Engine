using BH.Engine.Geometry;
using BH.Engine.Spatial;
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
        public static double RelationLength(this Graph graph, SpatialRelation spatialRelation)
        {
            double length = 0;
            if (spatialRelation.Curve != null)
                length = spatialRelation.Curve.ILength();
            else
            {
                IElement0D source = m_SpatialGraph.Entities[spatialRelation.Source] as IElement0D;
                IElement0D target = m_SpatialGraph.Entities[spatialRelation.Target] as IElement0D;
                length = source.IGeometry().Distance(target.IGeometry());
            }
            return length;
        }
    }
}
