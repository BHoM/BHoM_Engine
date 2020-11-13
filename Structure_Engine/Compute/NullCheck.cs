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
            if (bar == null)
            {
                ErrorMessage(methodName, "Bar");
                return false;
            }

            return bar.StartNode.NullCheck(methodName) && bar.EndNode.NullCheck(methodName);
        }

        [Description("Checks if an FEMesh or one of its Nodes are null and outputs relevant error message.")]
        [Input("mesh", "The FEMesh to test for null.")]
        [Input("nodeListIndices", "Optional list of nodes to limit check to")]
        [Input("methodName", "Optional name of the method to reference in the error message.")]
        [Output("pass", "A boolean which is true if the FEMeshFace passes the null check.")]
        public static bool NullCheck(this FEMesh mesh, List<int> nodeListIndices = default, string methodName = "Method")
        {
            if (mesh?.Nodes == null || mesh?.Nodes.Count == 0)
            {
                ErrorMessage(methodName, "FEMesh");
                return false;
            }

            if (nodeListIndices == default(List<int>) || nodeListIndices.Count == 0)
            {
                nodeListIndices = new List<int>();
                nodeListIndices.AddRange(Enumerable.Range(0, mesh.Nodes.Count - 1));
            }
            else if (mesh.Nodes.Count - 1 < nodeListIndices.Max())
            {
                Engine.Reflection.Compute.RecordError($"Cannot run {methodName} because Node indices are out of range for FEMesh");
                return false;
            }

            bool passes = true;
            for (int i = 0; i < nodeListIndices.Count && passes; i++)
            {
                passes = mesh.Nodes[nodeListIndices[i]].NullCheck(methodName);
            }

            return passes;
        }

        [Description("Checks if an FEMeshFace or one of its Nodes are null and outputs relevant error message.")]
        [Input("face", "The FEMeshFace to test for null.")]
        [Input("mesh", "The FEMesh to which the face belongs.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Output("pass", "A boolean which is true if the FEMeshFace passes the null check.")]
        public static bool NullCheck(this FEMeshFace face, FEMesh mesh, string methodName = "Method")
        {
            if (face?.NodeListIndices == null || face?.NodeListIndices.Count == 0)
            {
                ErrorMessage(methodName, "FEMeshFace");
                return false;
            }

            return mesh.NullCheck(face.NodeListIndices, methodName);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void ErrorMessage(string methodName, string type)
        {
            Engine.Reflection.Compute.RecordError($"Cannot run {methodName} because {type} failed a null check");
        }
    }
}

