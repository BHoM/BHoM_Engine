/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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


using BH.oM.Structure.Elements;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.oM.Base.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using BH.Engine.Analytical;
using BH.Engine.Structure;

namespace BH.Engine.Structure
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Converts a Panel with three or four control points to a femesh with a single Face. This is not a method to discretise a Panel, it simply converts a simple Panel to an identical feMesh.")]
        [Input("panel", "Panel to be converted to a feMesh.")]
        [Output("feMesh", "feMesh converted from a Panel.")]

        public static FEMesh PanelToFEMesh(this Panel panel)
        {
            if (panel.IsNull())
            {
                return null;
            }
            List<Point> points = new List<Point>();
            List<Edge> edges = panel.ExternalEdges;
            List<Face> faces = new List<Face>();
            if (panel.Openings.Count > 0)
            {
                Base.Compute.RecordError("This method does not support Panels with Openings");
                return null;
            }
            if (edges.Count > 4)
            {
                Base.Compute.RecordError("Panel contains more than 4 Edges");
                return null;
            }
            foreach (Edge edge in edges)
            {
                ICurve curve = edge.Curve;
                points.AddRange(Geometry.Convert.IToPolyline(curve).ControlPoints);
            }
            int count = points.Distinct().Count();
            Face face = new Face();
            if (count > 4)
            {
                Base.Compute.RecordError("Panel contains more than four control points.");
                return null;
            }
            if (count == 4)
            {
                face = Geometry.Create.Face(0, 1, 2, 3);
            }
            else if (count == 3)
            {
                face = Geometry.Create.Face(0, 1, 2);
            }
            faces.Add(face);
            Mesh mesh = Geometry.Create.Mesh(points.Distinct(), faces);
            FEMesh feMesh = new FEMesh();
            feMesh = Create.FEMesh(mesh, null, null, panel.Name);
            if (panel.Property != null)
            {
                feMesh.Property = panel.Property;
            }
            if (panel.Tags.Count > 0)
            {
                feMesh.Tags = panel.Tags;
            }
            return feMesh;

        }
        /***************************************************/

    }
}




