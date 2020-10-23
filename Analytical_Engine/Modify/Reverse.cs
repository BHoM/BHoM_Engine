using BH.Engine.Base;
using BH.Engine.Geometry;
using BH.oM.Analytical.Elements;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Analytical
{
    public static partial class Modify
    {
        /***************************************************/
        /****           Public Methods                  ****/
        /***************************************************/
        public static IRelation IReverse(this IRelation relation)
        {
            return Reverse(relation as dynamic);
        }
        /***************************************************/
        public static IRelation Reverse(this Relation relation)
        {
            return relation.FlipSourceTarget();
        }
        /***************************************************/
        public static IRelation Reverse(this SpatialRelation relation)
        {
            IRelation flip = relation.FlipSourceTarget();
            ICurve curve = relation.Curve.DeepClone();
            SpatialRelation spatialFlip = flip as SpatialRelation;
            spatialFlip.Curve = curve.IFlip();
            return spatialFlip;
        }
        /***************************************************/
        public static Graph Reverse(this Graph graph)
        {
            List<IRelation> reversed = new List<IRelation>();
            foreach (IRelation relation in graph.Relations)
                reversed.Add(relation.Reverse());

            graph.Relations = reversed;
            return graph;
        }
        
        /***************************************************/
        /**** Fallback Method                           ****/
        /***************************************************/
        public static IRelation Reverse(this IRelation relation)
        {

            return relation;
        }
        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/
        private static IRelation FlipSourceTarget(this IRelation relation)
        {
            Guid oldSource = relation.Source;
            Guid oldTarget = relation.Target;
            relation.Source = oldTarget;
            relation.Target = oldSource;
            relation.Subgraph.Reverse();

            return relation;
        }
    }
}
