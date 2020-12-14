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

using BH.Engine.Geometry;
using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.oM.Reflection.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Structure
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Checks if a Node is null and outputs relevant error message.")]
        [Input("node", "The Node to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Output("pass", "A boolean which is true if the bar passes the null check.")]
        public static bool NullCheck(this Node node, string methodName = "Method")
        {
            // Check Node and Position
            if (node?.Position == null)
            {
                ErrorMessage(methodName, "Node");
                return false;
            }

            return true;
        }

        /***************************************************/

        [Description("Checks if a Bar or one of its Nodes are null and outputs relevant error message.")]
        [Input("bar", "The Bar to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Output("pass", "A boolean which is true if the bar passes the null check.")]
        public static bool NullCheck(this Bar bar, string methodName = "Method")
        {
            // Check bar
            if (bar == null)
            {
                ErrorMessage(methodName, "Bar");
                return false;
            }

            // Check bar Nodes
            return bar.StartNode.NullCheck(methodName) && bar.EndNode.NullCheck(methodName);
        }

        [Description("Checks if an FEMesh or one of its Nodes are null and outputs relevant error message.")]
        [Input("mesh", "The FEMesh to test for null.")]
        [Input("methodName", "Optional name of the method to reference in the error message.")]
        [Input("checkFaces", "Optional bool to tell the method whether to check FEMeshFaces or not.")]
        [Input("checkNodes", "Optional bool to tell the method whether to check mesh Nodes or not.")]
        [Input("nodeListIndices", "Optional list of nodes to limit check to")]
        [Output("pass", "A boolean which is true if the FEMeshFace passes the null check.")]
        public static bool NullCheck(this FEMesh mesh, string methodName = "Method", bool checkFaces = true, bool checkNodes = true, List<int> nodeListIndices = null)
        {
            // Check FEMesh
            if (mesh?.Nodes == null || mesh?.Nodes.Count == 0 || mesh?.Faces == null || mesh?.Faces.Count == 0)
            {
                ErrorMessage(methodName, "FEMesh");
                return false;
            }

            // Make sure to check all mesh Nodes if none are specified
            // When called from the FEMeshFace version of this method, we limit the Node check to relevant Nodes
            if (checkNodes && (nodeListIndices == null || nodeListIndices.Count == 0))
            {
                nodeListIndices = new List<int>();
                nodeListIndices.AddRange(Enumerable.Range(0, mesh.Nodes.Count - 1));
            }
            // If mesh nodes are specified, check that they are in range
            else if (checkNodes && mesh.Nodes.Count - 1 < nodeListIndices.Max())
            {
                Engine.Reflection.Compute.RecordError($"Cannot evaluate {methodName} because Node indices are out of range for FEMesh");
                return false;
            }

            // Check Nodes, but only if checkNodes is set to true
            // When called from methods that run on the list of mesh FEMeshFaces, we only want a basic check as null checks will be performed individually for each face
            bool passes = true;
            for (int i = 0; checkNodes && passes && i < nodeListIndices.Count ; i++)
            {
                passes = mesh.Nodes[nodeListIndices[i]].NullCheck(methodName);
            }

            // Check FEMeshFaces, but only if checkFaces is set to true
            // When called from the FEMeshFace version of this method, we do not need to check FEMeshFaces, as the only relevant face has already been checked
            for (int i = 0; checkFaces && passes && i < mesh.Faces.Count; i++)
            {
                passes = mesh.Faces[i].NullCheck(methodName);
            }

            return passes;
        }

        [Description("Checks if an FEMeshFace or one of its Nodes in the FEMesh are null and outputs relevant error message.")]
        [Input("face", "The FEMeshFace to test for null.")]
        [Input("mesh", "The FEMesh to which the face belongs.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Output("pass", "A boolean which is true if the FEMeshFace passes the null check.")]
        public static bool NullCheck(this FEMeshFace face, FEMesh mesh, string methodName = "Method")
        {
            // Check FEMeshFace and relevant nodes in FEMesh
            return face.NullCheck(methodName) && mesh.NullCheck(methodName, false, true, face.NodeListIndices);
        }

        [Description("Checks if an FEMeshFace or its NodeListIndices is null and outputs relevant error message.")]
        [Input("face", "The FEMeshFace to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Output("pass", "A boolean which is true if the FEMeshFace passes the null check.")]
        public static bool NullCheck(this FEMeshFace face, string methodName = "Method")
        {
            // Check FEMeshFace
            if (face?.NodeListIndices == null || face?.NodeListIndices.Count == 0)
            {
                ErrorMessage(methodName, "FEMeshFace");
                return false;
            }

            return true;
        }

        [Description("Checks if a Panel or its ExternalEdges are null and outputs relevant error message.")]
        [Input("panel", "The Panel to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Output("pass", "A boolean which is true if the FEMeshFace passes the null check.")]
        public static bool NullCheck(this Panel panel, string methodName = "Method")
        {
            // Check Panel
            if (panel?.ExternalEdges == null || panel?.ExternalEdges.Count == 0)
            {
                ErrorMessage(methodName, "Panel");
                return false;
            }

            foreach (Edge edge in panel.ExternalEdges) 
            {
                if (edge?.Curve == null)
                {
                    ErrorMessage(methodName, "Edge");
                    return false;
                }
            }

            return true;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void ErrorMessage(string methodName, string type)
        {
            Engine.Reflection.Compute.RecordError($"Cannot evaluate {methodName} because {type} failed a null check");
        }
    }
}
