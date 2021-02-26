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
using System.Collections.Generic;
using System.Linq;
using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Structure
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Converts an FEMesh to a Panel, each MeshFace is converted to a Panel with identical properties and fragments.")]
        [Input("feMesh", "FEMesh to be converted to a Panel.")]
        [Output("panel", "Panel converted from a FEMesh.")]

        public static List<Panel> FEMeshToPanel(FEMesh feMesh)
        {
            if (feMesh == null)
            {
                Engine.Reflection.Compute.RecordWarning("Checks identify null FEMesh");
                return null;
            }
            if (feMesh.Nodes == null)
            {
                Engine.Reflection.Compute.RecordWarning("Checks identify null nodes");
                return null;
            }
            if (feMesh.Faces == null)
            {
                Engine.Reflection.Compute.RecordWarning("Checks identify null faces");
                return null;
            }
            foreach (Node node in feMesh.Nodes)
            {
                if (node == null)
                {
                    Engine.Reflection.Compute.RecordWarning("Checks identify null node");
                    return null;
                }
            }
            foreach (FEMeshFace face in feMesh.Faces)
            {
                if (face == null)
                {
                    Engine.Reflection.Compute.RecordWarning("Checks identify null face");
                    return null;
                }
            }
            List<Polyline> polylines = new List<Polyline>();
            List<Point> points = new List<Point>();

            foreach (FEMeshFace feMeshFace in feMesh.Faces)
            {
                foreach (int nodeIndex in feMeshFace.NodeListIndices)
                    points.Add(feMesh.Nodes[nodeIndex].Position);
                polylines.Add(BH.Engine.Geometry.Create.Polyline(points));
            }

            points.Add(feMesh.Nodes.First().Position);
            polylines.Add(BH.Engine.Geometry.Create.Polyline(points));

            List<Panel> panels = new List<Panel>();
            foreach (Polyline polyline in polylines)
            {
                Panel panel = Create.Panel(polyline, null, null, feMesh.Name);
                if (feMesh.Property != null)
                    panel.Property = feMesh.Property;
                if (feMesh.Fragments != null)
                    panel.Fragments = feMesh.Fragments;
                panel.CustomData = feMesh.CustomData;
                panels.Add(panel);
            }
            return panels;

        }

        /***************************************************/

    }
}
