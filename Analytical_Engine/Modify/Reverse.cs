/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using BH.Engine.Geometry;
using BH.oM.Analytical.Elements;
using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        [Description("Modifies an IRelation by reversing it.")]
        [Input("relation", "The IRelation to reverse.")]
        [Output("relation", "The reversed IRelation.")]

        public static IRelation IReverse(this IRelation relation)
        {
            return Reverse(relation as dynamic);
        }

        /***************************************************/

        [Description("Modifies a Relation by reversing it.")]
        [Input("relation", "The Relation to reverse.")]
        [Output("relation", "The reversed Relation.")]

        public static IRelation Reverse(this Relation relation)
        {
            return relation.FlipSourceTarget();
        }
        /***************************************************/

        [Description("Modifies a SpatialRelation by reversing it.")]
        [Input("relation", "The SpatialRelation to reverse.")]
        [Output("relation", "The reversed SpatialRelation.")]

        public static IRelation Reverse(this SpatialRelation relation)
        {
            IRelation flip = relation.FlipSourceTarget();
            ICurve curve = relation.Curve.DeepClone();
            SpatialRelation spatialFlip = flip as SpatialRelation;
            spatialFlip.Curve = curve.IFlip();
            return spatialFlip;
        }

        /***************************************************/

        [Description("Modifies a Graph by reversing all Relations within it.")]
        [Input("graph", "The Graph to reverse.")]
        [Output("graph", "The reversed Graph.")]

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
