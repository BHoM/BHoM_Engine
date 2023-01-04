/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using BH.Engine.Base;
using BH.oM.Analytical.Elements;
using BH.oM.Analytical.Fragments;
using BH.oM.Base;
using BH.oM.Geometry;
using BH.oM.Base.Attributes;
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

        [Description("Convert an IDependencyFragment to relations.")]
        [Input("dependency", "The IDependencyFragment to convert.")]
        [Input("owningEntity", "The Guid of the entity from where the fragment was extracted.")]
        [Output("relations", "Collection of the converted relations.")]
        public static List<IRelation> IToRelation(this IDependencyFragment dependency, Guid owningEntity)
        {
            return ToRelation(dependency as dynamic, owningEntity);
        }

        /***************************************************/

        [Description("Convert a SourcesDependencyFragment to relations.")]
        [Input("dependency", "The DependencyFragment to convert.")]
        [Input("owningEntity", "The Guid of the entity from where the fragment was extracted.")]
        [Output("relations", "Collection of the converted relations.")]
        public static List<IRelation> ToRelation(this SourcesDependencyFragment dependency, Guid owningEntity)
        {
            if(dependency == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot convert a null dependency to a collection of relations.");
                return new List<IRelation>();
            }

            List<IRelation> relations = new List<IRelation>();
            for(int i = 0; i < dependency.Sources.Count; i++)
            {
                ICurve curve = null;
                if (i < dependency.Curves.Count - 1)
                    curve = dependency.Curves[i];
                relations.Add(
                new Relation()
                {
                    Source = dependency.Sources[i],
                    Target = owningEntity,
                    Curve = curve,
                });
            }
            
            return relations;
        }

        /***************************************************/

        [Description("Convert a TargetsDependencyFragment to relations.")]
        [Input("dependency", "The DependencyFragment to convert.")]
        [Input("owningEntity", "The Guid of the entity from where the fragment was extracted.")]
        [Output("relations", "Collection of the converted relations.")]
        public static List<IRelation> ToRelation(this TargetsDependencyFragment dependency, Guid owningEntity)
        {
            if (dependency == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot convert a null dependency to a collection of relations.");
                return new List<IRelation>();
            }

            List<IRelation> relations = new List<IRelation>();
            for (int i = 0; i < dependency.Targets.Count; i++)
            {
                ICurve curve = null;
                if (i < dependency.Curves.Count - 1)
                    curve = dependency.Curves[i];
                relations.Add(
                new Relation()
                {
                    Source = owningEntity,
                    Target = dependency.Targets[i],
                    Curve = curve,

                });
            }

            return relations;
        }

        /***************************************************/
        /**** Public Methods non IDependency converts   ****/
        /***************************************************/

        [Description("Convert an IBHoMObject to a collection of relations.")]
        [Input("obj", "The IBHoMObject to convert.")]
        [Output("relations", "Collection of the converted relations.")]
        public static List<IRelation> ToRelation(this IBHoMObject obj)
        {
            if(obj == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot convert a null BHoM object to a relation.");
                return new List<IRelation>();
            }

            List<IRelation> relations = new List<IRelation>();

            List<IFragment> dependencyFragments = obj.GetAllFragments(typeof(IDependencyFragment));
            foreach (IDependencyFragment dependency in dependencyFragments)
                relations.AddRange(dependency.IToRelation(obj.BHoM_Guid));

            return relations;
        }

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
                    relations.Add(forward.DeepClone().IReverse());
                    break;
            }
            return relations;
        }
        /***************************************************/

        [Description("Convert a an ILink to a single forward direction relation.")]
        [Input("link", "The ILink to convert.")]
        [Output("relation", "The converted relation.")]
        public static IRelation ToRelation<TNode>(this ILink<TNode> link)
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



