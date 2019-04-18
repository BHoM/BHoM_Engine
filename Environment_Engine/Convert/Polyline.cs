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

using BH.oM.Environment.Elements;
using BH.Engine.Geometry;
using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Convert
    {
        /***************************************************/
        /****          public Methods                   ****/
        /***************************************************/

        [Description("BH.Engine.Environment.Convert.ToPolyline => Returns a Polyline representation of an Environment Edge")]
        [Input("edge", "An Environment Edge object")]
        [Output("BHoM Geometry Polyline")]
        public static Polyline ToPolyline(this Edge edge)
        {
            return edge.Curve.ICollapseToPolyline(BH.oM.Geometry.Tolerance.Angle);
        }

        [Description("BH.Engine.Environment.Convert.ToEdges => Returns a collection of Environment Edges from a BHoM Geomtry Polyline")]
        [Input("polyline", "A BHoM Geometry Polyline to be split into Environment Edges")]
        [Output("A collection of Environment Edges")]
        public static List<Edge> ToEdges(this Polyline polyline)
        {
            List<Edge> edges = new List<Edge>();

            List<Polyline> polylines = polyline.SplitAtPoints(polyline.DiscontinuityPoints());
            foreach(Polyline p in polylines)
            {
                Edge e = new Edge();
                e.Curve = p;
                edges.Add(e);
            }

            return edges;
        }

        [Description("BH.Engine.Environment.Convert.ToPolyline => Returns a Polyline representation of a collection of Environment Edges")]
        [Input("edges", "A collection of Environment Edge objects to convert into a single polyline")]
        [Output("BHoM Geometry Polyline")]
        public static Polyline ToPolyline(this List<Edge> edges)
        {
            List<Point> edgePoints = new List<Point>();
            foreach (Edge e in edges)
                edgePoints.AddRange(e.Curve.IDiscontinuityPoints());

            return BH.Engine.Geometry.Create.Polyline(edgePoints.CullDuplicates());
        }

        [Description("BH.Engine.Environment.Convert.ToPolyline => Returns the external boundary from an Environment Panel as a BHoM Geometry Polyline")]
        [Input("panel", "An Environment Panel to obtain the external boundary from")]
        [Output("BHoM Geometry Polyline")]
        public static Polyline ToPolyline(this Panel panel)
        {
            return panel.ExternalEdges.ToPolyline();
        }

        [Description("BH.Engine.Environment.Convert.ToPolyline => Returns the external boundary from an Environment Opening as a BHoM Geometry Polyline")]
        [Input("opening", "An Environment Opening to obtain the external boundary from")]
        [Output("BHoM Geometry Polyline")]
        public static Polyline ToPolyline(this Opening opening)
        {
            return opening.Edges.ToPolyline();
        }
    }
}
