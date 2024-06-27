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
using System.ComponentModel;
using BH.Engine.Spatial;
using BH.Engine.Analytical;
using System.Linq;

namespace BH.Engine.Structure
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Converts a Panel with three or four control points to a FEMesh with a single Face. This is not a method to discretise a Panel, it simply converts a simple Panel to an identical FEMesh.")]
        [Input("panel", "Panel to be converted to a FEMesh.")]
        [Input("tolerance", "Tolerance used to cull duplicates from the control points of the Panel outline.")]
        [Output("feMesh", "FEMesh converted from a Panel.")]
        [PreviousVersion("7.3", "BH.Engine.Structure.Convert.PanelToFEMesh(BH.oM.Structure.Elements.Panel)")]
        public static FEMesh PanelToFEMesh(this Panel panel, double tolerance = Tolerance.MacroDistance)
        {
            // Null and invalid checks
            if (panel.IsNull())
            {
                return null;
            }
            if (!panel.IsPlanar(true, tolerance))
            {
                Base.Compute.RecordError("Panel is not planar and therefore cannot be converted to an FEMesh.");
                return null;
            }
            if (panel.Openings.Count > 0)
            {
                Base.Compute.RecordError("This method does not support Panels with Openings.");
                return null;
            }

            Face face = new Face();
            if (panel.IsOutlineQuad())
            {
                face = Geometry.Create.Face(0, 1, 2, 3);
            }
            else if (panel.IsOutlineTriangular())
            {
                face = Geometry.Create.Face(0, 1, 2);
            }
            else
            {
                Base.Compute.RecordError("Panel is not a planar quadilateral or triangular.");
                return null;
            }

            PolyCurve outline = panel.OutlineCurve().Curves.IJoin()[0];
            List<Point> points = outline.DiscontinuityPoints();
            points = points.CullDuplicates(tolerance).ISortAlongCurve(outline);

            List<Face> faces = new List<Face>() { face };

            Mesh mesh = Geometry.Create.Mesh(points, faces);
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




