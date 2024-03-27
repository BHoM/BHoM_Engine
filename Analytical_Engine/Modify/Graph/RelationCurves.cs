/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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
using BH.oM.Analytical.Graph.Projection;
using BH.oM.Analytical.Graph;
using BH.oM.Analytical.Fragments;
using BH.oM.Dimensional;
using BH.oM.Geometry;
using BH.oM.Base.Attributes;
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
        public static Graph IRelationCurves(this Graph graph, IProjection projection)
        {
           
            RelationCurves(graph, projection as dynamic);
            return graph;
        }

        /***************************************************/

        [Description("Modifies a Graph by ensuring all relations have a representative ICurve. If no curve has been provided a line is created between the source and target entities.")]
        [Input("graph", "The Graph to modify.")]
        [Input("projection", "SpatialProjection of the Graph.")]
        [Output("graph", "The modified Graph where all relations have a representative ICurve.")]
        private static Graph RelationCurves(this Graph graph, SpatialProjection projection)
        {
            //these should set representationfragment on relations
            foreach (IRelation relation in graph.Relations)
            {
                if (relation.Curve == null)
                {
                    IElement0D source = graph.Entities[relation.Source] as IElement0D;
                    IElement0D target = graph.Entities[relation.Target] as IElement0D;
                    relation.Curve = new Line() { Start = source.IGeometry(), End = target.IGeometry() };
                }
            }
            return graph;
        }

        /***************************************************/
        private static void RelationCurves(this Graph graph, GraphicalProjection projection)
        {
            foreach (IRelation relation in graph.Relations)
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

        private static void RelationCurves(this Graph graph, IProjection projection)
        {
            Base.Compute.RecordError("Modify method RelationCurves for IProjection provided has not been implemented.");
            return;
        }

        /***************************************************/
    }
}




