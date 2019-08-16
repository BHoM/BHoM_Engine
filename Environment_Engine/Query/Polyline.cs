/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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

using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

using BH.oM.Environment;
using BH.oM.Environment.Elements;
using BH.Engine.Geometry;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /****          public Methods                   ****/
        /***************************************************/

        [Description("Returns a Polyline representation of an Environment Edge")]
        [Input("edge", "An Environment Edge object")]
        [Output("polyline", "BHoM Geometry Polyline")]
        public static Polyline Polyline(this Edge edge)
        {
            return edge.Curve.ICollapseToPolyline(BH.oM.Geometry.Tolerance.Angle);
        }

        [Description("Returns a Polyline representation of a collection of Environment Edges")]
        [Input("edges", "A collection of Environment Edge objects to convert into a single polyline")]
        [Output("polyline", "BHoM Geometry Polyline")]
        public static Polyline Polyline(this List<Edge> edges)
        {
            if (edges == null || edges.Count == 0)
                return null; //Cannot get a polyline from 0 edges...
            
            List<Point> edgePoints = new List<Point>();
            foreach (Edge e in edges)
                edgePoints.AddRange(e.Curve.IDiscontinuityPoints());

            edgePoints = edgePoints.CullDuplicates();
            edgePoints.Add(edgePoints.First()); //To close the polyline

            return BH.Engine.Geometry.Create.Polyline(edgePoints);
        }

        [Description("Returns the external boundary from an Environment Panel as a BHoM Geometry Polyline")]
        [Input("panel", "An Environment Panel to obtain the external boundary from")]
        [Output("polyline", "BHoM Geometry Polyline")]
        public static Polyline Polyline(this Panel panel)
        {
            return panel.ExternalEdges.Polyline();
        }

        [Description("Returns the external boundary from an Environment Opening as a BHoM Geometry Polyline")]
        [Input("opening", "An Environment Opening to obtain the external boundary from")]
        [Output("polyline", "BHoM Geometry Polyline")]
        public static Polyline Polyline(this Opening opening)
        {
            return opening.Edges.Polyline();
        }

        [Description("Returns the external boundary from a generic Environment Object")]
        [Input("environmentObject", "Any object implementing the IEnvironmentObject interface that can have its boundaries extracted")]
        [Output("polyline", "BHoM Geometry Polyline")]
        public static Polyline Polyline(this IEnvironmentObject environmentObject)
        {
            return Polyline(environmentObject as dynamic);
        }
    }
}
