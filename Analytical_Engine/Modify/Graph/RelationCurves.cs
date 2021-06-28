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
using BH.Engine.Spatial;
using BH.oM.Analytical.Elements;
using BH.oM.Analytical.Fragments;
using BH.oM.Base;
using BH.oM.Dimensional;
using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Analytical
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Modifies a Graph by ensuring all relations have a representative ICurve. If no curve has been provided a line is created between the source and target entities.")]
        [Input("graph", "The Graph to modify.")]
        [Input("projection", "The IProjection required of the Graph.")]
        [Output("graph", "The modified Graph where all relations have a representative ICurve.")]
        [PreviousVersion("4.3", "BH.Engine.Analytical.Modify.IRelationCurves(BH.oM.Analytical.Elements.Graph,BH.oM.Analytical.Elements.IProjection)")]

        public static Graph<T> IRelationCurves<T>(this Graph<T> graph, IProjection projection)
            where T : IBHoMObject
        {
           
            RelationCurves(graph, projection as dynamic);
            return graph;
        }

        /***************************************************/

        [Description("Modifies a Graph by ensuring all relations have a representative ICurve. If no curve has been provided a line is created between the source and target entities.")]
        [Input("graph", "The Graph to modify.")]
        [Input("projection", "SpatialProjection of the Graph.")]
        [Output("graph", "The modified Graph where all relations have a representative ICurve.")]
        private static Graph<T> RelationCurves<T>(this Graph<T> graph, SpatialProjection projection)
            where T : INode
        {
            foreach (IRelation<T> relation in graph.Relations)
            {
                if (relation.Curve == null)
                {
                    T source = graph.Entities[relation.Source];
                    T target = graph.Entities[relation.Target] ;
                    relation.Curve = new Line() { Start = source.Position, End = target.Position };
                }
            }
            return graph;
        }

        /***************************************************/
        private static void RelationCurves<T>(this Graph<T> graph, GraphicalProjection projection)
            where T : IBHoMObject
        {
            foreach (IRelation<T> relation in graph.Relations)
            {
                if (relation.Curve == null)
                {
                    ProjectionFragment sourceProjectionFrag = graph.Entities[relation.Source].FindFragment<ProjectionFragment>();
                    ProjectionFragment targetProjectionFrag = graph.Entities[relation.Target].FindFragment<ProjectionFragment>();
                    
                    //if(sourceProjectionFrag!= null && targetProjectionFrag!=null)  
                    //    relation.Curve = new Line() { Start = sourceProjectionFrag.Position, End = targetProjectionFrag.Position };
                }
            }
        }

        /***************************************************/
        /**** Fallback Methods                          ****/
        /***************************************************/

        private static void RelationCurves<T>(this Graph<T> graph, IProjection projection)
            where T : IBHoMObject
        {
            Reflection.Compute.RecordError("Modify method RelationCurves for IProjection provided has not been implemented.");
            return;
        }

        /***************************************************/
    }
}

