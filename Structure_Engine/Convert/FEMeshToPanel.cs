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


using BH.oM.Structure.Elements;
using BH.oM.Geometry;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Structure
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Converts an FEMesh to a Panel, each MeshFace is converted to a Panel with identical properties and fragments.")]
        [Input("feMesh", "FEMesh to be converted to a Panel.")]
        [Output("panel", "Panel converted from a FEMesh.")]

        public static List<Panel> FEMeshToPanel(this FEMesh feMesh)
        {
            if (feMesh.IsNull())
            {
                return null;
            }
            if (feMesh.Nodes.Count < 3)
            {
                Base.Compute.RecordError("Insufficient number of nodes to be able to convert FEMesh to a Panel.");
                return null;
            }
            if (feMesh.Faces.Count < 1)
            {
                Base.Compute.RecordError("At least one FEFace required to construct a Panel from FEMesh.");
                return null;
            }

            List<Polyline> polylines = new List<Polyline>();

            foreach (FEMeshFace feMeshFace in feMesh.Faces)
            {
                List<Point> points = new List<Point>();
                foreach (int nodeIndex in feMeshFace.NodeListIndices)
                {
                    points.Add(feMesh.Nodes[nodeIndex].Position);
                }
                points.Add(feMesh.Nodes[feMeshFace.NodeListIndices.First()].Position);
                polylines.Add(Geometry.Create.Polyline(points));
            }
            List<Panel> panels = new List<Panel>();
            Panel panel = new Panel();
            foreach (Polyline polyline in polylines)
            {
                panel = Create.Panel(polyline, name:feMesh.Name);
                if (feMesh.Property != null)
                {
                    panel.Property = feMesh.Property;
                }
                if (feMesh.Fragments.Count > 0)
                {
                    panel.Fragments = feMesh.Fragments;
                }
                if (feMesh.Tags.Count > 0)
                {
                    panel.Tags = feMesh.Tags;
                }
                panels.Add(panel);
            }
            return panels;
        }

        /***************************************************/

    }
}




