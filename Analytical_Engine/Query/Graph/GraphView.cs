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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Analytical
{
    public static partial class Query 
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a Graph view defined by the view provided.")]
        [Input("graph", "The Graph to query.")]
        [Input("view", "The required IView.")]
        [Output("graph", "The view of the original Graph.")]
        public static Graph IGraphView(this Graph graph, IView view)
        {
            return GraphView(graph, view as dynamic);
        }

        /***************************************************/

        [Description("Returns a Graph view that contains only spatial entities. Spatial entities are those implementing IElement0D.")]
        [Input("graph", "The Graph to query.")]
        [Input("view", "The SpatialView.")]
        [Output("graph", "The spatial Graph.")]
        private static Graph GraphView(this Graph graph, SpatialView view)
        {
            Graph spatialGraph = graph.DeepClone();
            spatialGraph.Entities = graph.FilterEntities(typeof(IElement0D)).DeepClone();
            Modify.IRelationCurves(spatialGraph, view);
            return spatialGraph;
        }

        /***************************************************/

        [Description("Returns a process view of the Graph.")]
        [Input("graph", "The Graph to query.")]
        [Input("view", "The ProcessView.")]
        [Output("graph", "The process Graph.")]
        private static Graph GraphView(this Graph graph, ProcessView view)
        {
            Graph processGraph = graph.DeepClone();
            foreach (IBHoMObject entity in processGraph.Entities.Values.ToList())
            {
                ProcessViewFragment viewFragment = entity.FindFragment<ProcessViewFragment>();
                //if no process view fragment, remove entity
                if (viewFragment == null)
                    processGraph.RemoveEntity(entity.BHoM_Guid);
                else
                {
                    //if all group names are in the ignore list remove entity
                    if(!view.GroupsToIgnore.Except(viewFragment.GroupNames).Any())
                        processGraph.RemoveEntity(entity.BHoM_Guid);
                }
            }
            Modify.ILayout(processGraph, view.Layout as dynamic);
            Modify.IRelationCurves(processGraph, view);
            return processGraph;
        }

        /***************************************************/
        /**** Fallback Methods                          ****/
        /***************************************************/

        private static Graph GraphView(this Graph graph, IView view)
        {
            Reflection.Compute.RecordError("IView provided does not have a corresponding GraphView method implemented.");
            return new Graph();
        }

        /***************************************************/
    }
}
