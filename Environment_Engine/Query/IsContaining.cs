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

using BH.Engine.Geometry;
using BH.Engine.Spatial;
using BH.Engine.Analytical;
using BH.oM.Analytical.Elements;
using BH.oM.Base.Attributes;
using BH.oM.Dimensional;
using BH.oM.Environment.Elements;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Defines whether an Environment Panel is contained by at least one group of panels representing spaces")]
        [Input("panelsAsSpaces", "A nested collection of Environment Panels representing spaces")]
        [Input("panel", "The Environment Panel to be checked to see if it is contained by the panelsAsSpaces")]
        [Output("isContaining", "True if the panel is contained by at least one group of panels, false if it is not")]
        public static bool IsContaining(this List<List<Panel>> panelsAsSpaces, Panel panel)
        {
            foreach (List<Panel> lst in panelsAsSpaces)
            {
                if (lst.Where(x => x.BHoM_Guid == panel.BHoM_Guid).FirstOrDefault() != null)
                    return true;
            }

            return false;
        }

        [Description("Defines whether an a BHoM Geometry Point is contained within a list of Points")]
        [Input("pts", "A collection of BHoM Geometry Points")]
        [Input("pt", "The point being checked to see if it is contained within the list of points")]
        [Output("isContaining", "True if the point is contained within the list, false if it is not")]
        public static bool IsContaining(this List<Point> pts, Point pt)
        {
            return (pts.Where(point => Math.Round(point.X, 3) == Math.Round(pt.X, 3) && Math.Round(point.Y, 3) == Math.Round(pt.Y, 3) && Math.Round(point.Z, 3) == Math.Round(pt.Z, 3)).FirstOrDefault() != null);
        }

        [Description("Defines whether an Environment Panel contains a provided point")]
        [Input("panel", "An Environment Panel to check with")]
        [Input("pt", "The point being checked to see if it is contained within the bounds of the panel")]
        [Input("acceptOnEdges", "Decide whether to allow the point to sit on the edge of the panel, default false")]
        [Output("isContaining", "True if the point is contained within the panel, false if it is not")]
        public static bool IsContaining(this Panel panel, Point pt, bool acceptOnEdges = false, double tolerance = BH.oM.Geometry.Tolerance.Distance)
        {
            if (panel == null || pt == null)
                return false;

            return new List<Panel> { panel }.IsContaining(pt, acceptOnEdges, tolerance);
        }

        [Description("Defines whether a collection of Environment Panels contains a provided point")]
        [Input("panels", "A collection of Environment Panels to check with")]
        [Input("point", "The point being checked to see if it is contained within the bounds of the panels")]
        [Input("acceptOnEdges", "Decide whether to allow the point to sit on the edge of the panel, default false")]
        [Output("isContaining", "True if the point is contained within the bounds of the panels, false if it is not")]
        public static bool IsContaining(this List<Panel> panels, Point point, bool acceptOnEdges = false, double tolerance = BH.oM.Geometry.Tolerance.Distance)
        {
            if (panels == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query if a collection of panels contains a point if the panels are null.");
                return false;
            }

            List<Polyline> polylines = new List<Polyline>();
            foreach (Panel be in panels)
                polylines.Add(be.Polyline());

            return Engine.Geometry.Query.IsContaining(polylines, new List<Point> (){ point }, acceptOnEdges, tolerance).First();
        }

        [Description("Defines whether a collection of Environment Panels contains each of a provided list of points.")]
        [Input("panels", "A collection of Environment Panels to check with.")]
        [Input("points", "The points to check to see if each point is contained within the bounds of the panels.")]
        [Input("acceptOnEdges", "Decide whether to allow the point to sit on the edge of the panel, default false.")]
        [Output("isContaining", "True if the point is contained within the bounds of the panels, false if it is not for each point provided.")]
        public static List<bool> IsContaining(this List<Panel> panels, List<Point> points, bool acceptOnEdges = false, double tolerance = BH.oM.Geometry.Tolerance.Distance)
        {
            if (panels == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query if a collection of panels contains a point if the panels are null.");
                return new List<bool>() { false };
            }

            List<Polyline> polylines = new List<Polyline>();
            foreach (Panel be in panels)
                polylines.Add(be.Polyline());

            return Engine.Geometry.Query.IsContaining(polylines, points, acceptOnEdges, tolerance);
        }


        [Description("Defines whether an Environment Space contains a provided point.")]
        [Input("space", "An Environment Space object defining a perimeter to build a 3D volume from and check if the volume contains the provided point.")]
        [Input("spaceHeight", "The height of the space.", typeof(BH.oM.Quantities.Attributes.Length))]
        [Input("points", "The points being checked to see if it is contained within the bounds of the 3D volume.")]
        [Input("acceptOnEdges", "Decide whether to allow the point to sit on the edge of the space, default false.")]
        [Output("isContaining", "True if the point is contained within the space, false if it is not.")]
        public static List<bool> IsContaining(this Space space, double spaceHeight, List<Point> points, bool acceptOnEdges = false)
        {
            return BH.Engine.Analytical.Query.IsContaining(space, spaceHeight, points, acceptOnEdges);
        }
    }
}