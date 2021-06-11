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

using BH.Engine.Geometry;
using BH.oM.Geometry;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Loads;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.SectionProperties.Reinforcement;
using BH.oM.Structure.SurfaceProperties;
using BH.oM.Reflection.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [PreviousVersion("4.2", "BH.Engine.Structure.Compute.NullCheck(BH.oM.Structure.Elements.Node, System.String)")]
        [Description("Checks if a Node or its defining properties are null and outputs relevant error message.")]
        [Input("node", "The Node to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Output("isNull", "True if the Node or its defining properties are null.")]
        public static bool IsNull(this Node node, string msg = "", [CallerMemberName] string methodName = "Method")
        {
            // Check Node and Position
            if (node == null)
            {
                ErrorMessage(methodName, "Node", msg);
                return true;
            }
            else if (node.Position == null)
            {
                ErrorMessage(methodName, "Node Position", msg);
                return true;
            }

            return false;
        }

        /***************************************************/

        [PreviousVersion("4.2", "BH.Engine.Structure.Compute.NullCheck(BH.oM.Structure.Elements.Bar, System.String)")]
        [Description("Checks if a Bar or its defining properties are null and outputs relevant error message.")]
        [Input("bar", "The Bar to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Output("isNull", "True if the Bar or its defining properties are null.")]
        public static bool IsNull(this Bar bar, string msg = "", [CallerMemberName] string methodName = "Method")
        {
            // Check bar
            if (bar == null)
            {
                ErrorMessage(methodName, "Bar", msg);
                return true;
            }
            else if (bar.StartNode.IsNull("The Node (StartNode) is owned by a Bar.", methodName) || bar.EndNode.IsNull("The Node (EndNode) is owned by a Bar.", methodName))
                return true;

            return false;
        }

        [PreviousVersion("4.2", "BH.Engine.Structure.Compute.NullCheck(BH.oM.Structure.Elements.FEMesh, System.String, System.Boolean, System.Boolean, System.Collections.Generic.List<System.Int32>)")]
        [Description("Checks if an FEMesh or its defining properties are null and outputs relevant error message.")]
        [Input("mesh", "The FEMesh to test for null.")]
        [Input("methodName", "Optional name of the method to reference in the error message.")]
        [Input("checkFaces", "Optional bool to tell the method whether to check FEMeshFaces or not.")]
        [Input("checkNodes", "Optional bool to tell the method whether to check mesh Nodes or not.")]
        [Input("nodeListIndices", "Optional list of nodes to limit check to.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Output("isNull", "True if the FEMesh or its defining properties are null.")]
        public static bool IsNull(this FEMesh mesh, bool checkFaces = true, bool checkNodes = true, List<int> nodeListIndices = null, string msg = "", [CallerMemberName] string methodName = "Method")
        {
            // Check FEMesh
            if (mesh == null)
            {
                ErrorMessage(methodName, "FEMesh", msg);
                return true;
            }
            else if (mesh.Faces == null || mesh.Faces.Count == 0)
            {
                Reflection.Compute.RecordError($"Cannot evaluate {methodName} because the list of Faces are null or the number of Faces is 0. {msg}");
                return true;
            }
            else if (mesh.Nodes == null || mesh.Nodes.Count == 0)
            {
                Reflection.Compute.RecordError($"Cannot evaluate {methodName} because the list of Nodes are null or the number of Nodes is 0. {msg}");
                return true;
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
                Reflection.Compute.RecordError($"Cannot evaluate {methodName} because Node indices are out of range for FEMesh");
                return false;
            }

            // Check Nodes, but only if checkNodes is set to true
            // When called from methods that run on the list of mesh FEMeshFaces, we only want a basic check as null checks will be performed individually for each face
            bool isNull = false;
            for (int i = 0; checkNodes && !isNull && i < nodeListIndices.Count; i++)
            {
                isNull = mesh.Nodes[nodeListIndices[i]].IsNull($"The Node is owned by an FEMesh. {msg}", methodName);
            }

            // Check FEMeshFaces, but only if checkFaces is set to true
            // When called from the FEMeshFace version of this method, we do not need to check FEMeshFaces, as the only relevant face has already been checked
            for (int i = 0; checkFaces && !isNull && i < mesh.Faces.Count; i++)
            {
                isNull = mesh.Faces[i].IsNull($"The FEMeshFace is owned by an FEMesh. {msg}", methodName);
            }

            return isNull;
        }

        [PreviousVersion("4.2", "BH.Engine.Structure.Compute.NullCheck(BH.oM.Structure.Elements.FEMeshFace, BH.oM.Structure.Elements.FEMesh, System.String)")]
        [Description("Checks if an FEMeshFace or its defining properties are null and outputs relevant error message.")]
        [Input("face", "The FEMeshFace to test for null.")]
        [Input("mesh", "The FEMesh to which the face belongs.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Output("isNull", "True if the FEMeshFace or its defining properties are null.")]
        public static bool IsNull(this FEMeshFace face, FEMesh mesh, string msg = "", [CallerMemberName] string methodName = "Method")
        {
            // Check FEMeshFace and relevant nodes in FEMesh
            if (face.IsNull(msg, methodName))
            {
                return true;
            }
            else if (mesh.IsNull(false, true, face.NodeListIndices, msg, methodName))
            {
                return true;
            }

            return false;
        }

        [PreviousVersion("4.2", "BH.Engine.Structure.Compute.NullCheck(BH.oM.Structure.Elements.FEMeshFace, System.String)")]
        [Description("Checks if an FEMeshFace or its defining properties are null and outputs relevant error message.")]
        [Input("face", "The FEMeshFace to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Output("isNull", "True if the FEMeshFace or its defining properties are null.")]
        public static bool IsNull(this FEMeshFace face, string msg = "", [CallerMemberName] string methodName = "Method")
        {
            // Check FEMeshFace
            if (face == null)
            {
                ErrorMessage(methodName, "FEMeshFace", msg);
                return true;
            }
            else if (face.NodeListIndices == null)
            {
                ErrorMessage(methodName, "FEMeshFace NodeListIndicies", msg);
                return true;
            }
            else if (face.NodeListIndices.Count == 0)
            {
                Reflection.Compute.RecordError($"Cannot evaluate {methodName} because the Face NodeListIndicies count is 0. {msg}");
                return true;
            }

            return false;
        }

        [PreviousVersion("4.2", "BH.Engine.Structure.Compute.NullCheck(BH.oM.Structure.Elements.Panel, System.String)")]
        [Description("Checks if a Panel or its defining properties are null and outputs relevant error message.")]
        [Input("panel", "The Panel to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Output("isNull", "True if the Panel or its defining properties are null.")]
        public static bool IsNull(this Panel panel, string msg = "", [CallerMemberName] string methodName = "Method")
        {
            if (panel == null)
            {
                ErrorMessage(methodName, "Panel", msg);
                return true;
            }
            else if (panel.ExternalEdges == null)
            {
                ErrorMessage(methodName, "Panel ExternalEdges", msg);
                return true;
            }
            else if (panel.ExternalEdges.Count == 0)
            {
                Reflection.Compute.RecordError($"Cannot evaluate {methodName} because the Panel ExternalEdges count is 0. {msg}");
                return true;
            }
            else if (panel.ExternalEdges.Any(x => x.IsNull($"The ExternalEdges are owned by a Panel. {msg}", methodName)))
            {
                if (!string.IsNullOrEmpty(msg))
                    Reflection.Compute.RecordError(msg);
                return true;
            }

            return false;
        }

        [Description("Checks if a Surface or its defining properties are null and outputs relevant error message.")]
        [Input("surface", "The Surface to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Output("isNull", "True if the Surface or its defining properties are null.")]
        public static bool IsNull(this Surface surface, string msg = "", [CallerMemberName] string methodName = "Method")
        {
            if (surface == null)
            {
                ErrorMessage(methodName, "Surface", msg);
                return true;
            }
            else if (surface.Extents == null)
            {
                ErrorMessage(methodName, "Surface Extents", msg);
                return true;
            }


            return false;
        }

        [PreviousVersion("4.2", "BH.Engine.Structure.Compute.NullCheck(BH.oM.Structure.Elements.Edge, System.String)")]
        [Description("Checks if a Edge or its defining properties are null and outputs relevant error message.")]
        [Input("edge", "The Edge to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Output("isNull", "True if the Edge or its defining properties are null.")]
        public static bool IsNull(this Edge edge, string msg = "", [CallerMemberName] string methodName = "Method")
        {
            if (edge == null)
            {
                ErrorMessage(methodName, "Edge", msg);
                return true;
            }
            else if (edge.Curve == null)
            {
                ErrorMessage(methodName, "Edge Curve", msg);
                return true;
            }

            return false;
        }

        [Description("Checks if a SectionProperty is null and outputs relevant error message.")]
        [Input("sectionProperty", "The SectionProperty to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Output("isNull", "True if the SectionProperty is null.")]
        public static bool IsNull(this ISectionProperty sectionProperty, string msg = "", [CallerMemberName] string methodName = "Method")
        {
            if (sectionProperty == null)
            {
                ErrorMessage(methodName, "SectionProperty", msg);
                return true;
            }

            return false;
        }

        [Description("Checks if a SurfaceProperty is null and outputs relevant error message.")]
        [Input("surfaceProperty", "The SurfaceProperty to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Output("isNull", "True if the SurfaceProperty is null.")]
        public static bool IsNull(this ISurfaceProperty surfaceProperty, string msg = "", [CallerMemberName] string methodName = "Method")
        {
            if (surfaceProperty == null)
            {
                ErrorMessage(methodName, "SurfaceProperty", msg);
                return true;
            }

            return false;
        }

        [Description("Checks if a MaterialFragment is null and outputs relevant error message.")]
        [Input("material", "The MaterialFragment to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Output("isNull", "True if the MaterialFragment is null.")]
        public static bool IsNull(this IMaterialFragment material, string msg = "", [CallerMemberName] string methodName = "Method")
        {
            if (material == null)
            {
                ErrorMessage(methodName, "MaterialFragment", msg);
                return true;
            }

            return false;
        }

        [Description("Checks if a BarReinforcement is null and outputs relevant error message.")]
        [Input("reinforcement", "The BarReinforcement to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Output("isNull", "True if the BarReinforcement is null.")]
        public static bool IsNull(this IBarReinforcement reinforcement, string msg = "", [CallerMemberName] string methodName = "Method")
        {
            if (reinforcement == null)
            {
                ErrorMessage(methodName, "BarReinforcement", msg);
                return true;
            }

            return false;
        }

        [Description("Checks if a Reinforcement is null and outputs relevant error message.")]
        [Input("reinforcement", "The Reinforcement to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Output("isNull", "True if the Reinforcement is null.")]
        public static bool IsNull(this Reinforcement reinforcement, string msg = "", [CallerMemberName] string methodName = "Method")
        {
            if (reinforcement == null)
            {
                ErrorMessage(methodName, "Reinforcement", msg);
                return true;
            }

            return false;
        }

        [Description("Checks if a Load is null and outputs relevant error message.")]
        [Input("load", "The Load to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Output("isNull", "True if the Load is null.")]
        public static bool IsNull(this ILoad load, string msg = "", [CallerMemberName] string methodName = "Method")
        {
            if (load == null)
            {
                ErrorMessage(methodName, "Load", msg);
                return true;
            }

            return false;
        }

        [Description("Checks if a Constraint6DOF is null and outputs relevant error message.")]
        [Input("constraint", "The Constraint6DOF to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Output("isNull", "True if the Constraint6DOF is null.")]
        public static bool IsNull(this Constraint6DOF constraint, string msg = "", [CallerMemberName] string methodName = "Method")
        {
            if (constraint == null)
            {
                ErrorMessage(methodName, "Constraint6DOF", msg);
                return true;
            }

            return false;
        }

        [Description("Checks if a Constraint3DOF is null and outputs relevant error message.")]
        [Input("constraint", "The Constraint3DOF to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Output("isNull", "True if the Constraint3DOF is null.")]
        public static bool IsNull(this Constraint3DOF constraint, string msg = "", [CallerMemberName] string methodName = "Method")
        {
            if (constraint == null)
            {
                ErrorMessage(methodName, "Constraint3DOF", msg);
                return true;
            }

            return false;
        }

        [Description("Checks if a Constraint4DOF is null and outputs relevant error message.")]
        [Input("constraint", "The Constraint4DOF to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Output("isNull", "True if the Constraint4DOF is null.")]
        public static bool IsNull(this Constraint4DOF constraint, string msg = "", [CallerMemberName] string methodName = "Method")
        {
            if (constraint == null)
            {
                ErrorMessage(methodName, "Constraint4DOF", msg);
                return true;
            }

            return false;
        }

        [Description("Checks if a Case is null and outputs relevant error message.")]
        [Input("loadCase", "The Case to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Output("isNull", "True if the Case is null.")]
        public static bool IsNull(this ICase loadCase, [CallerMemberName] string methodName = "Method", string msg = "")
        {
            if (loadCase == null)
            {
                ErrorMessage(methodName, "Case", msg);
                return true;
            }

            return false;
        }

        /***************************************************/
        /**** Public Methods - Interface                ****/
        /***************************************************/

        [Description("Checks if an AreaElement is null and outputs relevant error message.")]
        [Input("element", "The AreaElement to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Output("pass", "True if the AreaElement is null.")]
        public static bool IIsNull(this IAreaElement element, string msg = "", [CallerMemberName] string methodName = "Method")
        {
            if (element == null)
            {
                ErrorMessage(methodName, "AreaElement", msg);
                return true;
            }

            if (element is Panel)
                return IsNull(element as Panel, msg, methodName);
            else if (element is FEMesh)
                return IsNull(element as FEMesh, true, true, null, msg, methodName);

            return false;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void ErrorMessage(string methodName = "Method", string type = "type", string msg = "")
        {
            Reflection.Compute.RecordError($"Cannot evaluate {methodName} because the {type} is null. {msg}");
        }

        /***************************************************/
        /**** Private Methods - Fallback                ****/
        /***************************************************/

        private static bool IsNull(this IAreaElement element, string msg = "", [CallerMemberName] string methodName = "Method")
        {
            if (element == null)
            {
                ErrorMessage(methodName, "AreaElement", msg);
                return true;
            }

            return false;
        }

    }
}