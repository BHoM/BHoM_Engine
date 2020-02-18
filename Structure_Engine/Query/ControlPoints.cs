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

using System.Collections.Generic;
using System.Linq;
using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.Engine.Geometry;
using BH.oM.Reflection.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /******************************************/
        /****      Element control points      ****/
        /******************************************/

        [Description("Gets all control points from the edgecurves of a Panel. Control point extraction will depend on the type of curve in the edges. \n" + 
                     "Method will return point from only the external edges or from external edges as well as openings depending on input setting")]
        [Input("panel", "The Panel to extract the control points from")]
        [Input("externalOnly", "If true, only controlpoints from the external edges is extracted. If false, external edges as well as opening controlpoints are included.")]
        [Output("points","The extracted control points")]
        public static List<Point> ControlPoints(this Panel panel, bool externalOnly = false)
        {
            List<Point> pts = panel.ExternalEdges.ControlPoints();
            if (!externalOnly)
            {
                foreach (Opening o in panel.Openings)
                {
                    pts.AddRange(o.ControlPoints());
                }
            }
            return pts;
        }

        /******************************************/

        [Description("Gets all control points from the edgecurves of an Opening. Control point extraction will depend on the type of curve in the edges.")]
        [Input("opening", "The Opening to extract the control points from")]
        [Output("points", "The extracted control points")]
        public static List<Point> ControlPoints(this Opening opening)
        {
            return opening.Edges.ControlPoints();
        }

        /******************************************/

        [Description("Gets all control points from a collection of Edges. Control point extraction will depend on the type of curve in the edges.")]
        [Input("panel", "The Edges to extract the control points from")]
        [Output("points", "The extracted control points")]
        public static List<Point> ControlPoints(this List<Edge> edges)
        {
            List<Point> pts = edges.SelectMany(e => e.Curve.IControlPoints()).ToList();
            return pts;
        }

        /******************************************/

        [Description("Gets the start and end node positions of a Bar")]
        [Input("panel", "The Bar to extract the control points from")]
        [Output("points", "The extracted control points")]
        public static List<Point> ControlPoints(this Bar bar)
        {
            return new List<Point> { bar.StartNode.Position.Clone(), bar.EndNode.Position.Clone() };
        }

        /******************************************/
    }
}

