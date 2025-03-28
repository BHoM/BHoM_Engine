/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Geometry;
using BH.oM.Environment;
using BH.oM.Environment.Elements;
using BH.Engine.Geometry;

using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a collection of Environment Edges from a BHoM Geomtry Polyline")]
        [Input("polyline", "A BHoM Geometry Polyline to be split into Environment Edges")]
        [Output("edges", "A collection of Environment Edges")]
        public static List<Edge> ToEdges(this Polyline polyline)
        {
            if (polyline == null)
                return null;

            List<Edge> edges = new List<Edge>();

            List<Polyline> polylines = polyline.SplitAtPoints(polyline.DiscontinuityPoints());
            foreach (Polyline p in polylines)
            {
                Edge e = new Edge();
                e.Curve = p;
                edges.Add(e);
            }

            return edges;
        }

        [Description("Returns a collection of Environment Edges from a collection of BHoM Geomtry Polylines")]
        [Input("polylines", "A collection of BHoM Geometry Polylines to be split into Environment Edges")]
        [Output("edges", "A collection of Environment Edges")]
        public static List<Edge> ToEdges(this List<Polyline> polylines)
        {
            if (polylines == null)
                return null;

            List<Edge> edges = new List<Edge>();

            foreach (Polyline p in polylines)
                edges.AddRange(p.ToEdges());

            return edges;
        }
    }
}





