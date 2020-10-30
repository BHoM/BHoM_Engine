using BH.Engine.Base;
using BH.Engine.Spatial;
using BH.oM.Analytical.Elements;
using BH.oM.Analytical.Fragments;
using BH.oM.Dimensional;
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
        /**** Public Methods                            ****/
        /***************************************************/
        public static void IRelationCurves(this Graph graph, IView view)
        {
            RelationCurves(graph, view as dynamic);
        }

        /***************************************************/
        private static void RelationCurves(this Graph graph, SpatialView view)
        {
            foreach (IRelation relation in graph.Relations)
            {
                if (relation.Curve == null)
                {
                    IElement0D source = graph.Entities[relation.Source] as IElement0D;
                    IElement0D target = graph.Entities[relation.Target] as IElement0D;
                    relation.Curve = new Line() { Start = source.IGeometry(), End = target.IGeometry() };
                }
            }
        }

        /***************************************************/
        private static void RelationCurves(this Graph graph, ProcessView view)
        {
            foreach (IRelation relation in graph.Relations)
            {
                if (relation.Curve == null)
                {
                    EntityViewFragment sourceViewFrag = graph.Entities[relation.Source].FindFragment<EntityViewFragment>();
                    EntityViewFragment targetViewFrag = graph.Entities[relation.Target].FindFragment<EntityViewFragment>();
                    
                    if(sourceViewFrag!= null && targetViewFrag!=null)  
                        relation.Curve = new Line() { Start = sourceViewFrag.Position, End = targetViewFrag.Position };
                }
            }
        }
        /***************************************************/
        /**** Fallback Methods                          ****/
        /***************************************************/

        private static void RelationCurves(this Graph graph, IView view)
        {
            Reflection.Compute.RecordError("Modify method RelationCurves for IView provided has not been implemented.");
            return;
        }

        /***************************************************/
    }
}
