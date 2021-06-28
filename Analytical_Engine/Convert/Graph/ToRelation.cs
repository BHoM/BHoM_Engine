/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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

        [Description("Convert an IDependencyFragment to relations.")]
        [Input("dependency", "The IDependencyFragment to convert.")]
        [Input("owningEntity", "The Guid of the entity from where the fragment was extracted.")]
        [Output("relations", "Collection of the converted relations.")]
        [PreviousVersion("4.3", "BH.Engine.Analytical.Convert.IToRelation(BH.oM.Analytical.Fragments.IDependencyFragment, System.Guid)")]
        public static List<IRelation<T>> IToRelation<T>(this IDependencyFragment dependency, Guid owningEntity)
            where T : IBHoMObject
        {
            return ToRelation(dependency as dynamic, owningEntity);
        }

        /***************************************************/

        [Description("Convert a SourcesDependencyFragment to relations.")]
        [Input("dependency", "The DependencyFragment to convert.")]
        [Input("owningEntity", "The Guid of the entity from where the fragment was extracted.")]
        [Output("relations", "Collection of the converted relations.")]
        [PreviousVersion("4.3", "BH.Engine.Analytical.Convert.IToRelation(BH.oM.Analytical.Fragments.SourcesDependencyFragment, System.Guid)")]
        public static List<IRelation<T>> ToRelation<T>(this SourcesDependencyFragment dependency, Guid owningEntity)
            where T : IBHoMObject
        {
            if(dependency == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot convert a null dependency to a collection of relations.");
                return new List<IRelation<T>>();
            }

            List<IRelation<T>> relations = new List<IRelation<T>>();
            for(int i = 0; i < dependency.Sources.Count; i++)
            {
                ICurve curve = null;
                if (i < dependency.Curves.Count - 1)
                    curve = dependency.Curves[i];
                relations.Add(
                new Relation<T>()
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
        [PreviousVersion("4.3", "BH.Engine.Analytical.Convert.IToRelation(BH.oM.Analytical.Fragments.TargetsDependencyFragment, System.Guid)")]
        public static List<IRelation<T>> ToRelation<T>(this TargetsDependencyFragment dependency, Guid owningEntity)
            where T : IBHoMObject
        {
            if (dependency == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot convert a null dependency to a collection of relations.");
                return new List<IRelation<T>>();
            }

            List<IRelation<T>> relations = new List<IRelation<T>>();
            for (int i = 0; i < dependency.Targets.Count; i++)
            {
                ICurve curve = null;
                if (i < dependency.Curves.Count - 1)
                    curve = dependency.Curves[i];
                relations.Add(
                new Relation<T>()
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
        [PreviousVersion("4.3", "BH.Engine.Analytical.Convert.IToRelation(BH.oM.Base.IBHoMObject)")]
        public static List<IRelation<T>> ToRelation<T>(this T obj)
            where  T : IBHoMObject
        {
            if(obj == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot convert a null BHoM object to a relation.");
                return new List<IRelation<T>>();
            }

            List<IRelation<T>> relations = new List<IRelation<T>>();

            List<IFragment> dependencyFragments = obj.GetAllFragments(typeof(IDependencyFragment));
            foreach (IDependencyFragment dependency in dependencyFragments)
                relations.AddRange(ToRelation<T>(dependency as dynamic, obj.BHoM_Guid));

            return relations;
        }

        /***************************************************/

        [Description("Convert an ILink to a collection of relations.")]
        [Input("link", "The ILink to convert.")]
        [Input("linkDirection", "The optional RelationDirection defining the direction of the relation. Default is RelationDirection.Forwards.")]
        [Output("relations", "Collection of the converted relations.")]
        [PreviousVersion("4.3", "BH.Engine.Analytical.Convert.IToRelation(BH.oM.Analytical.Elements.ILink,BH.oM.Analytical.Elements.RelationDirection)")]
        public static List<IRelation<TNode>> ToRelation<TNode>(this ILink<TNode> link, RelationDirection linkDirection = RelationDirection.Forwards)
            where TNode : INode
        {
            List<IRelation<TNode>> relations = new List<IRelation<TNode>>();
            IRelation<TNode> forward = ToRelation<TNode>(link);
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
        [PreviousVersion("4.3", "BH.Engine.Analytical.Convert.IToRelation(BH.oM.Analytical.Elements.ILink)")]
        public static IRelation<TNode> ToRelation<TNode>(this ILink<TNode> link)
            where TNode : INode
        {
            Relation<TNode> relation = new Relation<TNode>()
            {
                Source = link.StartNode.BHoM_Guid,
                Target = link.EndNode.BHoM_Guid,
                Curve = (ICurve)link.IGeometry(),
            };

            Graph<TNode> subgraph = new Graph<TNode>();
            subgraph.Entities.Add(link.StartNode.BHoM_Guid, link.StartNode);
            subgraph.Entities.Add(link.EndNode.BHoM_Guid, link.EndNode);
            //subgraph.Entities.Add(link.BHoM_Guid, link);
            subgraph.Relations.Add(new Relation<TNode>() { Source = link.StartNode.BHoM_Guid, Target = link.BHoM_Guid });
            subgraph.Relations.Add(new Relation<TNode>() { Source = link.BHoM_Guid, Target = link.StartNode.BHoM_Guid });
            relation.Subgraph = subgraph;

            return relation;
        }

        /***************************************************/
        /**** Fallback Methods                          ****/
        /***************************************************/

        private static List<IRelation<T>> ToRelation<T>(this IDependencyFragment dependency, Guid owningEntity)
            where T : IBHoMObject
        {
            // Do nothing
            List<IRelation<T>> relations = new List<IRelation<T>>();
            
            return relations;
        }

        /***************************************************/
    }
}

