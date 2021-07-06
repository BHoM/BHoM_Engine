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
using BH.oM.Graphics.Views;
using BH.oM.Graphics.Scales;
using BH.oM.Reflection.Attributes;
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
        [PreviousVersion("4.2", "BH.Engine.Analytical.Modify.IProjectGraph(BH.oM.Analytical.Elements.Graph,BH.oM.Analytical.Elements.IProjection)")]
        public static Graph<T> IProjectGraph<T>(this Graph<T> graph, IProjection projection)
            where T : IBHoMObject
        {
            Graph<T> graphProjected = ProjectGraph(graph, projection as dynamic);

            return graphProjected;
        }
        /***************************************************/

        [Description("Returns a Graph projection that contains only geometric entities. Spatial entities are those implementing IElement0D.")]
        [Input("graph", "The Graph to query.")]
        [Input("projection", "The SpatialView.")]
        [Output("graph", "The spatial Graph.")]
        [PreviousVersion("4.2", "BH.Engine.Analytical.Modify.IProjectGraph(BH.oM.Analytical.Elements.Graph,BH.oM.Analytical.Elements.GeometricProjection)")]
        private static Graph<T> ProjectGraph<T>(this Graph<T> graph, GeometricProjection projection)
            where T : IBHoMObject
        {
            Graph<T> geometricGraph = graph.DeepClone();
            foreach (T entity in geometricGraph.Entities.Values.ToList())
            {
                if (!typeof(IElement0D).IsAssignableFrom(entity.GetType()))
                    geometricGraph.RemoveEntity(entity.BHoM_Guid);
            }

            return geometricGraph;
        }

        /***************************************************/

        [Description("Returns a Graph projection that contains only spatial entities. Spatial entities are those implementing IElement0D.")]
        [Input("graph", "The Graph to query.")]
        [Input("projection", "The SpatialView.")]
        [Output("graph", "The spatial Graph.")]
        [PreviousVersion("4.2", "BH.Engine.Analytical.Modify.IProjectGraph(BH.oM.Analytical.Elements.Graph,BH.oM.Analytical.Elements.SpatialProjection)")]
        private static Graph<T> ProjectGraph<T>(this Graph<T> graph, SpatialProjection projection)
            where T : IBHoMObject
        {
            Graph<T> spatialGraph = graph.DeepClone();
            //set representation based on projection
            
            return spatialGraph;
        }

        /***************************************************/

        [Description("Returns a process projection of the Graph.")]
        [Input("graph", "The Graph to query.")]
        [Input("projection", "The ProcessView.")]
        [Output("graph", "The process Graph.")]
        [PreviousVersion("4.2", "BH.Engine.Analytical.Modify.IProjectGraph(BH.oM.Analytical.Elements.Graph,BH.oM.Analytical.Elements.GraphicalProjection)")]

        private static Graph<T> ProjectGraph<T>(this Graph<T> graph,  GraphicalProjection projection)
            where T : IBHoMObject
        {
            Graph<T> processGraph = graph.DeepClone();

            Dataset graphData = SetGraphDataSet(processGraph, projection.View);

            Graphics.Modify.IView(projection.View, graphData);

            processGraph.Fragments.AddOrReplace(graphData.FindFragment<GraphRepresentation>());
            return processGraph;
        }

        /***************************************************/
        /**** Fallback Methods                          ****/
        /***************************************************/

        private static Graph<T> ProjectGraph<T>(this Graph<T> graph, IProjection projection)
            where T : IBHoMObject
        {
            Reflection.Compute.RecordError("IProjection provided does not have a corresponding GraphView method implemented.");
            return new Graph<T>();
        }
       
        /***************************************************/

        private static Dataset SetGraphDataSet<T>(Graph<T> graph, IView view)
            where T : IBHoMObject
        {
            if(view is DependencyChart)
            {
                DependencyChart chart = view as DependencyChart;
                string groupPropertName = chart.Boxes.Group;
                foreach (T entity in graph.Entities())
                {
                    if (chart.Boxes.GroupsToIgnore.Contains(entity.PropertyValue(groupPropertName)))
                        graph.RemoveEntity(entity);
                }
            }

            BHoMGroup<T> entities = new BHoMGroup<T>();
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

