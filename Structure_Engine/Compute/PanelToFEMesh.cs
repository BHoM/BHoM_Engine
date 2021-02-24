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


using BH.oM.Structure.Elements;
using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using BH.Engine.Analytical;

namespace BH.Engine.Structure
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Converting panel with 3 or 4 control points to FEmesh ")]
        [Input("panel", "Panel to be converted to an FEMesh.")]
        [Output("FEMesh", "FEMesh converted from a Panel.")]

        public static FEMesh PanelToFEMesh(Panel panel)
        {
            List<Point> points = new List<Point>();
            List<Edge> edges = panel.ExternalEdges;
            List<Face> faces = new List<Face>();
            if (panel.Openings.Count()>0)
            {
                Engine.Reflection.Compute.RecordWarning("PanelToFEMesh does not support panel with opening");
                return null;
            }
            if (panel.ExternalEdges == null)
                {
                Engine.Reflection.Compute.RecordWarning("Checks identify no External edges");
                return null;
            }
            if (panel == null)
            {
                Engine.Reflection.Compute.RecordWarning("Checks identify null panel");
                return null;
            }
            foreach (Edge edge in edges)
            {
                ICurve curve = BH.Engine.Analytical.Query.Geometry(edge);


                points.AddRange(BH.Engine.Geometry.Convert.IToPolyline(curve).ControlPoints);
            }
            int Count = points.Distinct().Count();
            Face face = new Face();
            if (Count == 4)
            {
                face = Geometry.Create.Face(0, 1, 2, 3);
            }
            else if (Count == 3)
            {
                face = Geometry.Create.Face(0, 1, 2);
            }
            else
            {
                Engine.Reflection.Compute.RecordWarning("PanelToFEMesh does not support panel more than 4 edges");
                return null;
            }
            faces.Add(face);
            Mesh mesh = BH.Engine.Geometry.Create.Mesh(points.Distinct(), faces);
            FEMesh fEMesh = new FEMesh();
            if (panel.Property != null)
            {
                fEMesh = BH.Engine.Structure.Create.FEMesh(mesh, panel.Property, null, panel.Name);
                return fEMesh;
            }
            else
            {
                fEMesh = BH.Engine.Structure.Create.FEMesh(mesh, null, null, panel.Name);
                Engine.Reflection.Compute.RecordWarning("Panels don't have any Section Property input");
                return fEMesh;
            }
        }

        /***************************************************/

    }
}

