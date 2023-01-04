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
using BH.Engine.Spatial;
using BH.oM.Analytical.Elements;
using BH.oM.Analytical.Fragments;
using BH.oM.Base;
using BH.oM.Dimensional;
using BH.oM.Geometry;
using BH.oM.Graphics.Views;
using BH.oM.Graphics.Scales;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BH.Engine.Reflection;
using BH.oM.Graphics.Fragments;
using BH.oM.Data.Library;

namespace BH.Engine.Analytical
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a Graph projection defined by the projection provided.")]
        [Input("graph", "The Graph to query.")]
        [Input("projection", "The required IView.")]
        [Output("graph", "The projection of the original Graph.")]
        public static Graph IProjectGraph(this Graph graph, IProjection projection)
        {
            Graph graphProjected = ProjectGraph(graph, projection as dynamic);

            return graphProjected;
        }
        /***************************************************/

        [Description("Returns a Graph projection that contains only geometric entities. Spatial entities are those implementing IElement0D.")]
        [Input("graph", "The Graph to query.")]
        [Input("projection", "The SpatialView.")]
        [Output("graph", "The spatial Graph.")]
        private static Graph ProjectGraph(this Graph graph, GeometricProjection projection)
        {
            Graph geometricGraph = new Graph
            {
                BHoM_Guid = graph.BHoM_Guid,
                CustomData = graph.CustomData,
                Fragments = graph.Fragments,
                Name = graph.Name,
                Tags = graph.Tags
            };

            geometricGraph.Entities = graph.Entities.Where(x => x.Value is IElement0D).ToDictionary(x => x.Key, x => x.Value);
            HashSet<Guid> filteredEntityIds = new HashSet<Guid>(geometricGraph.Entities.Keys);
            geometricGraph.Relations = graph.Relations.Where(x => filteredEntityIds.Contains(x.Target) && filteredEntityIds.Contains(x.Source)).ToList();

            return geometricGraph;
        }

        /***************************************************/

        [Description("Returns a Graph projection that contains only spatial entities. Spatial entities are those implementing IElement0D.")]
        [Input("graph", "The Graph to query.")]
        [Input("projection", "The SpatialView.")]
        [Output("graph", "The spatial Graph.")]
        private static Graph ProjectGraph(this Graph graph, SpatialProjection projection)
        {
            Graph spatialGraph = graph.DeepClone();
            //set representation based on projection
            
            return spatialGraph;
        }

        /***************************************************/

        [Description("Returns a process projection of the Graph.")]
        [Input("graph", "The Graph to query.")]
        [Input("projection", "The ProcessView.")]
        [Output("graph", "The process Graph.")]

        private static Graph ProjectGraph(this Graph graph,  GraphicalProjection projection)
        {
            Graph processGraph = graph.DeepClone();

            Dataset graphData = SetGraphDataSet(processGraph, projection.View);

            Graphics.Modify.IView(projection.View, graphData);

            processGraph.Fragments.AddOrReplace(graphData.FindFragment<GraphRepresentation>());
            return processGraph;
        }

        /***************************************************/
        /**** Fallback Methods                          ****/
        /***************************************************/

        private static Graph ProjectGraph(this Graph graph, IProjection projection)
        {
            Base.Compute.RecordError("IProjection provided does not have a corresponding GraphView method implemented.");
            return new Graph();
        }
       
        /***************************************************/

        private static Dataset SetGraphDataSet(Graph graph, IView view)
        {
            if(view is DependencyChart)
            {
                DependencyChart chart = view as DependencyChart;
                string groupPropertName = chart.Boxes.Group;
                foreach (IBHoMObject entity in graph.Entities())
                {
                    if (chart.Boxes.GroupsToIgnore.Contains(entity.PropertyValue(groupPropertName)))
                        graph.RemoveEntity(entity);
                }
            }

            BHoMGroup<IBHoMObject> entities = new BHoMGroup<IBHoMObject>();
            entities.Elements = graph.Entities();
            entities.Name = "Entities";

            BHoMGroup<IBHoMObject> relations = new BHoMGroup<IBHoMObject>();
            relations.Elements = graph.Relations.Cast<IBHoMObject>().ToList();
            relations.Name = "Relations";

            Dataset graphData = new Dataset();
            graphData.Data = new List<IBHoMObject>(); 
            graphData.Data.Add(entities);
            graphData.Data.Add(relations);

            return graphData;

        }
    }
}



