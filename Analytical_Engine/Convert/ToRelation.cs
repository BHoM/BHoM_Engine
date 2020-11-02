using BH.Engine.Base;
using BH.oM.Analytical.Elements;
using BH.oM.Analytical.Fragments;
using BH.oM.Base;
using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Analytical
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Convert an IDependencyFragment assigned to relations")]
        [Input("dependency", "The IDependencyFragment to convert.")]
        [Input("owningEntity", "The Guid of the entity from where the fragment was extracted.")]
        [Output("relations", "Collection of the converted relations.")]

        public static List<IRelation> IToRelation(this IDependencyFragment dependency, Guid owningEntity)
        {
            return ToRelation(dependency as dynamic, owningEntity);
        }

        /***************************************************/

        [Description("Convert a DependencyFragment assigned to relations")]
        [Input("dependency", "The DependencyFragment to convert.")]
        [Input("owningEntity", "The Guid of the entity from where the fragment was extracted.")]
        [Output("relations", "Collection of the converted relations.")]

        public static List<IRelation> ToRelation(this DependencyFragment dependency, Guid owningEntity)
        {
            List<IRelation> relations = new List<IRelation>();
            relations.Add(
                new Relation() { 
                Source = dependency.Source, 
                Target = dependency.Target,
                Processes = dependency.Processes,
                Curve = dependency.Curve,

            });
            return relations;
        }
        
        /***************************************************/
        /**** Public Methods non IDependency converts   ****/
        /***************************************************/

        [Description("Convert an ILink to a collection of relations.")]
        [Input("link", "The ILink to convert.")]
        [Input("linkDirection", "The optional RelationDirection defining the direction of the relation. Default is RelationDirection.Forwards.")]
        [Output("relations", "Collection of the converted relations.")]

        public static List<IRelation> ToRelation<TNode>(this ILink<TNode> link, RelationDirection linkDirection = RelationDirection.Forwards)
            where TNode : INode
        {
            List<IRelation> relations = new List<IRelation>();
            IRelation forward = ToRelation(link);
            switch (linkDirection)
            {
                case RelationDirection.Forwards:
                    relations.Add(forward);
                    break;
                case RelationDirection.Backwards:
                    relations.Add(forward.Reverse());
                    break;
                case RelationDirection.Both:
                    relations.Add(forward); 
                    relations.Add(forward.Reverse());
                    break;
            }
            return relations;
        }
        /***************************************************/

        [Description("Convert a collection of ILinks to a collection of relations.")]
        [Input("links", "The collection of ILinks to convert.")]
        [Input("linkDirection", "The optional RelationDirection defining the direction of the relation. Default is RelationDirection.Forwards.")]
        [Output("relations", "Collection of the converted relations.")]
        //UI cannot handle the list of generic inputs yet

        public static List<IRelation> ToRelation<TNode>(this List<ILink<TNode>> links, RelationDirection linkDirection)
            where TNode : INode
        {
            List<IRelation> relations = new List<IRelation>();
            links.ForEach(lnk => lnk.ToRelation(linkDirection));
            return relations;
        }

        /***************************************************/

        [Description("Convert a an ILink to a single forward direction relation.")]
        [Input("link", "The ILink to convert.")]
        [Output("relation", "The converted relation.")]

        private static IRelation ToRelation<TNode>(this ILink<TNode> link)
            where TNode : INode
        {
            Relation relation = new Relation()
            {
                Source = link.StartNode.BHoM_Guid,
                Target = link.EndNode.BHoM_Guid,
                Curve = (ICurve)link.IGeometry(),
            };

            Graph subgraph = new Graph();
            subgraph.Entities.Add(link.StartNode.BHoM_Guid, link.StartNode);
            subgraph.Entities.Add(link.EndNode.BHoM_Guid, link.EndNode);
            subgraph.Entities.Add(link.BHoM_Guid, link);
            subgraph.Relations.Add(new Relation() { Source = link.StartNode.BHoM_Guid, Target = link.BHoM_Guid });
            subgraph.Relations.Add(new Relation() { Source = link.BHoM_Guid, Target = link.StartNode.BHoM_Guid });
            relation.Subgraph = subgraph;

            return relation;
        }

        /***************************************************/
        /**** Fallback Methods                          ****/
        /***************************************************/

        private static List<IRelation> ToRelation(this IDependencyFragment dependency, Guid owningEntity)
        {
            // Do nothing
            List<IRelation> relations = new List<IRelation>();
            
            return relations;
        }

        /***************************************************/
    }
}
