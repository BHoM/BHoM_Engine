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

using BH.Engine.Spatial;
using BH.oM.Analytical.Elements;
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

        [Description("Returns a Graph that contains only spatial entities and relations. Spatial entities are those implementing IElement0D and spatial relations are those implementing SpatialRelation.")]
        [Input("graph", "The graph to search.")]
        [Output("graph", "The spatial graph.")]

        public static Graph SpatialGraph(this Graph graph)
        {
            Graph spatialGraph = new Graph();
            spatialGraph.Entities = graph.FilterEntities(typeof(IElement0D));
            spatialGraph.Relations = graph.FilterRelations(typeof(SpatialRelation));
            spatialGraph.CreateMissingCurves();
            return spatialGraph;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void CreateMissingCurves(this Graph graph)
        {
            foreach(SpatialRelation spatialRelation in graph.Relations)
            {
                if(spatialRelation.Curve == null)
                {
                    IElement0D source = graph.Entities[spatialRelation.Source] as IElement0D;
                    IElement0D target = graph.Entities[spatialRelation.Target] as IElement0D;
                    spatialRelation.Curve = new Line() { Start = source.IGeometry(), End = target.IGeometry() };
                }
            }
        }
    }
}
