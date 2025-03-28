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

using System.Collections.Generic;
using BH.oM.Environment.Elements;
using BH.oM.Geometry;
using BH.Engine.Geometry;

using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a collection of Environment Edges from a BHoM Geomtry Polyline")]
        [Input("curve", "A BHoM Geometry ICurve to be split into Environment Edges")]
        [Output("edges", "A collection of Environment Edges")]
        public static List<Edge> ToEdges(this ICurve curve)
        {
            return curve.ICollapseToPolyline(BH.oM.Geometry.Tolerance.Angle).ToEdges();
        }

        [Description("Returns a collection of Environment Edges from a BHoM Geomtry Polyline")]
        [Input("curves", "A collection of BHoM Geometry ICurve to be split into Environment Edges")]
        [Output("edges", "A collection of Environment Edges")]
        public static List<Edge> ToEdges(this List<ICurve> curves)
        {
            List<Edge> edges = new List<Edge>();
            foreach (ICurve c in curves)
                edges.AddRange(c.ToEdges());

            return edges;
        }
    }
}






