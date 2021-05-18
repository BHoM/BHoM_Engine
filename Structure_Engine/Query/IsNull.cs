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

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        
        [PreviousVersion("4.2", "BH.Engine.Structure.Compute.IsNull(BH.oM.Structure.Elements.Node, System.String)")]
        [Description("Checks if a Node or its defining properties are null and outputs relevant error message.")]
        [Input("node", "The Node to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("errorOverride", "Optional error message to override the default error message. Only the contents of this string will be returned as an error.")]
        [Output("isNull", "True if the Node or its defining properties are null.")]
        public static bool IsNull(this Node node, string methodName = "Method", string errorOverride = "")
        {
            // Check Node and Position
            if (node == null)
            {
                if(string.IsNullOrEmpty(errorOverride))
                    ErrorMessage(methodName, "Node");
                else
                    Reflection.Compute.RecordError(errorOverride);
                return true;
            }
            else if(node.Position == null)
            {
                if (string.IsNullOrEmpty(errorOverride))
                    ErrorMessage(methodName, "Node Position");
                else
                    Reflection.Compute.RecordError(errorOverride);

                return true;
            }

            return false;
        }

        /***************************************************/

        [PreviousVersion("4.2", "BH.Engine.Structure.Compute.IsNull(BH.oM.Structure.Elements.Bar, System.String)")]
        [Description("Checks if a Bar or its defining properties are null and outputs relevant error message.")]
        [Input("bar", "The Bar to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("errorOverride", "Optional error message to override the default error message. Only the contents of this string will be returned as an error.")]
        [Output("isNull", "True if the Bar or its defining properties are null.")]
        public static bool IsNull(this Bar bar, string methodName = "Method", string errorOverride = "")
        {
            // Check bar
            if (bar == null)
            {
                if (string.IsNullOrEmpty(errorOverride))
                    ErrorMessage(methodName, "Bar");
                else
                    Reflection.Compute.RecordError(errorOverride);
                return true;
            }
            else if (bar.StartNode.IsNull(methodName, $"Cannot evaluate {methodName}, StartNode or its Position is null") 
                || bar.EndNode.IsNull(methodName, $"Cannot evaluate {methodName}, StartNode or its Position is null"))
                return true;

            return false;
        }

        [PreviousVersion("4.2", "BH.Engine.Structure.Compute.IsNull(BH.oM.Structure.Elements.FEMesh, System.String, System.Boolean, System.Boolean, System.Collections.Generic.List<System.Int32>)")]
        [Description("Checks if an FEMesh or its defining properties are null and outputs relevant error message.")]
        [Input("mesh", "The FEMesh to test for null.")]
        [Input("methodName", "Optional name of the method to reference in the error message.")]
        [Input("checkFaces", "Optional bool to tell the method whether to check FEMeshFaces or not.")]
        [Input("checkNodes", "Optional bool to tell the method whether to check mesh Nodes or not.")]
        [Input("nodeListIndices", "Optional list of nodes to limit check to")]
        [Input("errorOverride", "Optional error message to override the default error message. Only the contents of this string will be returned as an error.")]
        [Output("isNull", "True if the FEMesh or its defining properties are null.")]
        public static bool IsNull(this FEMesh mesh, string methodName = "Method", bool checkFaces = true, bool checkNodes = true, List<int> nodeListIndices = null, string errorOverride = "")
        {
            // Check FEMesh
            if(mesh == null)
            {
                if (string.IsNullOrEmpty(errorOverride))
                    ErrorMessage(methodName, "Bar");
                else
                    Reflection.Compute.RecordError(errorOverride);
                return true;
            }
            else if(mesh.Faces == null || mesh.Faces.Count == 0)
            {
                if (string.IsNullOrEmpty(errorOverride))
                    Reflection.Compute.RecordError($"Cannot evaluate {methodName} because the list of Faces are null or the number of Faces is 0.");
                else
                    Reflection.Compute.RecordError(errorOverride);
                return true;
            }
            else if(mesh.Nodes == null || mesh.Nodes.Count == 0 )
            {
                if (string.IsNullOrEmpty(errorOverride))
                    Reflection.Compute.RecordError($"Cannot evaluate {methodName} because the list of Nodes are null or the number of Nodes is 0.");
                else
                    Reflection.Compute.RecordError(errorOverride);
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
            bool passes = false;
            for (int i = 0; checkNodes && passes && i < nodeListIndices.Count; i++)
            {
                passes = mesh.Nodes[nodeListIndices[i]].IsNull(methodName, $"Cannot evaluate {methodName} because at least one of the FEMesh Nodes or its Position is null.");
            }

            // Check FEMeshFaces, but only if checkFaces is set to true
            // When called from the FEMeshFace version of this method, we do not need to check FEMeshFaces, as the only relevant face has already been checked
            for (int i = 0; checkFaces && passes && i < mesh.Faces.Count; i++)
            {
                passes = mesh.Faces[i].IsNull(methodName, $"Cannot evaluate {methodName} because at least one of the FEMeshFaces or its NodeListIndicies are null or count is equal to 0.");
            }

            return passes;
        }

        [PreviousVersion("4.2", "BH.Engine.Structure.Compute.IsNull(BH.oM.Structure.Elements.FEMeshFace, BH.oM.Structure.Elements.FEMesh, System.String)")]
        [Description("Checks if an FEMeshFace or its defining properties are null and outputs relevant error message.")]
        [Input("face", "The FEMeshFace to test for null.")]
        [Input("mesh", "The FEMesh to which the face belongs.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("errorOverride", "Optional error message to override the default error message. Only the contents of this string will be returned as an error.")]
        [Output("isNull", "True if the FEMeshFace or its defining properties are null.")]
        public static bool IsNull(this FEMeshFace face, FEMesh mesh, string methodName = "Method", string errorOverride = "")
        {
            // Check FEMeshFace and relevant nodes in FEMesh
            if (face.IsNull(methodName))
            {
                if (!string.IsNullOrEmpty(errorOverride))
                    Reflection.Compute.RecordError(errorOverride);
                return true;
            }
            else if (mesh.IsNull(methodName, false, true, face.NodeListIndices))
            {
                if (!string.IsNullOrEmpty(errorOverride))
                    Reflection.Compute.RecordError(errorOverride);
                return true;
            }

            return false;   
        }

        [PreviousVersion("4.2", "BH.Engine.Structure.Compute.IsNull(BH.oM.Structure.Elements.FEMeshFace, System.String)")]
        [Description("Checks if an FEMeshFace or its defining properties are null and outputs relevant error message.")]
        [Input("face", "The FEMeshFace to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("errorOverride", "Optional error message to override the default error message. Only the contents of this string will be returned as an error.")]
        [Output("isNull", "True if the FEMeshFace or its defining properties are null.")]
        public static bool IsNull(this FEMeshFace face, string methodName = "Method", string errorOverride = "")
        {
            // Check FEMeshFace
            if(face == null)
            {
                if (string.IsNullOrEmpty(errorOverride))
                    ErrorMessage(methodName, "FEMeshFace");
                else
                    Reflection.Compute.RecordError(errorOverride);
                return true;
            }
            else if (face.NodeListIndices == null)
            {
                if (string.IsNullOrEmpty(errorOverride))
                    ErrorMessage(methodName, "FEMeshFace NodeListIndicies");
                else
                    Reflection.Compute.RecordError(errorOverride);
                return true;
            }
            else if(face.NodeListIndices.Count == 0)
            {
                if (string.IsNullOrEmpty(errorOverride))
                    Reflection.Compute.RecordError($"Cannot evaluate { methodName} because the Face NodeListIndicies count is 0.");
                else
                    Reflection.Compute.RecordError(errorOverride);
                return true;
            }

            return false;
        }

        [PreviousVersion("4.2", "BH.Engine.Structure.Compute.IsNull(BH.oM.Structure.Elements.Panel, System.String)")]
        [Description("Checks if a Panel or its defining properties are null and outputs relevant error message.")]
        [Input("panel", "The Panel to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("errorOverride", "Optional error message to override the default error message. Only the contents of this string will be returned as an error.")]
        [Output("isNull", "True if the Panel or its defining properties are null.")]
        public static bool IsNull(this Panel panel, string methodName = "Method", string errorOverride = "")
        {
            if(panel == null)
            {
                if (string.IsNullOrEmpty(errorOverride))
                    ErrorMessage(methodName, "Panel");
                else
                    Reflection.Compute.RecordError(errorOverride);
                return true;
            }
            else if(panel.ExternalEdges == null)
            {
                if (string.IsNullOrEmpty(errorOverride))
                    ErrorMessage(methodName, "Panel ExternalEdges");
                else
                    Reflection.Compute.RecordError(errorOverride);
                return true;
            }
            else if(panel.ExternalEdges.Count == 0)
            {
                if (string.IsNullOrEmpty(errorOverride))
                    Reflection.Compute.RecordError($"Cannot evaluate { methodName} because the Panel ExternalEdges count is 0.");
                else
                    Reflection.Compute.RecordError(errorOverride);
                return true;
            }   
            else if(panel.ExternalEdges.Any(x => x.IsNull(methodName, $"Cannot evaluate { methodName} because at least one of the Panels ExternalEdges or its Curve is null.")))
            {
                if (!string.IsNullOrEmpty(errorOverride))
                    Reflection.Compute.RecordError(errorOverride);
                return true;
            }

            return false;
        }

        [Description("Checks if a Surface or its defining properties are null and outputs relevant error message.")]
        [Input("panel", "The Surface to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("errorOverride", "Optional error message to override the default error message. Only the contents of this string will be returned as an error.")]
        [Output("isNull", "True if the Surface or its defining properties are null.")]
        public static bool IsNull(this Surface surface, string methodName = "Method", string errorOverride = "")
        {
            if (surface == null)
            {
                if (string.IsNullOrEmpty(errorOverride))
                    ErrorMessage(methodName, "Surface");
                else
                    Reflection.Compute.RecordError(errorOverride);
                return true;
            }

            return false;
        }

        [PreviousVersion("4.2", "BH.Engine.Structure.Compute.IsNull(BH.oM.Structure.Elements.Edge, System.String)")]
        [Description("Checks if a Edge or its defining properties are null and outputs relevant error message.")]
        [Input("panel", "The Edge to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("errorOverride", "Optional error message to override the default error message. Only the contents of this string will be returned as an error.")]
        [Output("isNull", "True if the Edge or its defining properties are null.")]
        public static bool IsNull(this Edge edge, string methodName = "Method", string errorOverride = "")
        {
            if(edge == null)
            {
                if (string.IsNullOrEmpty(errorOverride))
                    ErrorMessage(methodName, "Edge");
                else
                    Reflection.Compute.RecordError(errorOverride);
                return true;
            }
            else if(edge.Curve == null)
            {
                if (string.IsNullOrEmpty(errorOverride))
                    ErrorMessage(methodName, "Edge Curve");
                else
                    Reflection.Compute.RecordError(errorOverride);
                return true;
            }

            return false;
        }

        [Description("Checks if a SectionProperty is null and outputs relevant error message.")]
        [Input("panel", "The SectionProperty to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("errorOverride", "Optional error message to override the default error message. Only the contents of this string will be returned as an error.")]
        [Output("isNull", "True if the SectionProperty is null.")]
        public static bool IsNull(this ISectionProperty sectionProperty, string methodName = "Method", string errorOverride = "")
        {
            if (sectionProperty == null)
            {
                if (string.IsNullOrEmpty(errorOverride))
                    ErrorMessage(methodName, "SectionProperty");
                else
                    Reflection.Compute.RecordError(errorOverride);
                return true;
            }

            return false;
        }

        [Description("Checks if a SurfaceProperty is null and outputs relevant error message.")]
        [Input("panel", "The SurfaceProperty to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("errorOverride", "Optional error message to override the default error message. Only the contents of this string will be returned as an error.")]
        [Output("isNull", "True if the SurfaceProperty is null")]
        public static bool IsNull(this ISurfaceProperty surfaceProperty, string methodName = "Method", string errorOverride = "")
        {
            if (surfaceProperty == null)
            {
                if (string.IsNullOrEmpty(errorOverride))
                    ErrorMessage(methodName, "SurfaceProperty");
                else
                    Reflection.Compute.RecordError(errorOverride);
                return true;
            }

            return false;
        }

        [Description("Checks if a MaterialFragment is null and outputs relevant error message.")]
        [Input("panel", "The MaterialFragment to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("errorOverride", "Optional error message to override the default error message. Only the contents of this string will be returned as an error.")]
        [Output("isNull", "True if the MaterialFragment is null.")]
        public static bool IsNull(this IMaterialFragment material, string methodName = "Method", string errorOverride = "")
        {
            if (material == null)
            {
                if (string.IsNullOrEmpty(errorOverride))
                    ErrorMessage(methodName, "MaterialFragment");
                else
                    Reflection.Compute.RecordError(errorOverride);
                return true;
            }

            return false;
        }

        [Description("Checks if a BarReinforcement is null and outputs relevant error message.")]
        [Input("panel", "The BarReinforcement to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("errorOverride", "Optional error message to override the default error message. Only the contents of this string will be returned as an error.")]
        [Output("isNull", "True if the BarReinforcement is null.")]
        public static bool IsNull(this IBarReinforcement reinforcement, string methodName = "Method", string errorOverride = "")
        {
            if (reinforcement == null)
            {
                if (string.IsNullOrEmpty(errorOverride))
                    ErrorMessage(methodName, "BarReinforcement");
                else
                    Reflection.Compute.RecordError(errorOverride);
                return true;
            }

            return false;
        }

        [Description("Checks if a Load is null and outputs relevant error message.")]
        [Input("panel", "The Load to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("errorOverride", "Optional error message to override the default error message. Only the contents of this string will be returned as an error.")]
        [Output("isNull", "True if the Load is null.")]
        public static bool IsNull(this ILoad load, string methodName = "Method", string errorOverride = "")
        {
            if (load == null)
            {
                if (string.IsNullOrEmpty(errorOverride))
                    ErrorMessage(methodName, "Load");
                else
                    Reflection.Compute.RecordError(errorOverride);
                return true;
            }

            return false;
        }

        [Description("Checks if a Constraint6DOF is null and outputs relevant error message.")]
        [Input("panel", "The Constraint6DOF to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("errorOverride", "Optional error message to override the default error message. Only the contents of this string will be returned as an error.")]
        [Output("isNull", "True if the Constraint6DOF is null.")]
        public static bool IsNull(this Constraint6DOF constraint, string methodName = "Method", string errorOverride = "")
        {
            if (constraint == null)
            {
                if (string.IsNullOrEmpty(errorOverride))
                    ErrorMessage(methodName, "Constraint6DOF");
                else
                    Reflection.Compute.RecordError(errorOverride);
                return true;
            }

            return false;
        }

        [Description("Checks if a Constraint3DOF is null and outputs relevant error message.")]
        [Input("panel", "The Constraint3DOF to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("errorOverride", "Optional error message to override the default error message. Only the contents of this string will be returned as an error.")]
        [Output("isNull", "True if the Constraint3DOF is null.")]
        public static bool IsNull(this Constraint3DOF constraint, string methodName = "Method", string errorOverride = "")
        {
            if (constraint == null)
            {
                if (string.IsNullOrEmpty(errorOverride))
                    ErrorMessage(methodName, "Constraint3DOF");
                else
                    Reflection.Compute.RecordError(errorOverride);
                return true;
            }

            return false;
        }

        [Description("Checks if a Constraint4DOF is null and outputs relevant error message.")]
        [Input("panel", "The Constraint4DOF to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("errorOverride", "Optional error message to override the default error message. Only the contents of this string will be returned as an error.")]
        [Output("isNull", "True if the Constraint4DOF is null.")]
        public static bool IsNull(this Constraint4DOF constraint, string methodName = "Method", string errorOverride = "")
        {
            if (constraint == null)
            {
                if (string.IsNullOrEmpty(errorOverride))
                    ErrorMessage(methodName, "Constraint4DOF");
                else
                    Reflection.Compute.RecordError(errorOverride);
                return true;
            }

            return false;
        }

        [Description("Checks if a Case is null and outputs relevant error message.")]
        [Input("panel", "The Case to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("errorOverride", "Optional error message to override the default error message. Only the contents of this string will be returned as an error.")]
        [Output("isNull", "True if the Case is null.")]
        public static bool IsNull(this ICase loadCase, string methodName = "Method", string errorOverride = "")
        {
            if (loadCase == null)
            {
                if (string.IsNullOrEmpty(errorOverride))
                    ErrorMessage(methodName, "Case");
                else
                    Reflection.Compute.RecordError(errorOverride);
                return true;
            }

            return false;
        }

        /***************************************************/
        /**** Public Methods - Interface                ****/
        /***************************************************/

        [Description("Checks if an AreaElement is null and outputs relevant error message.")]
        [Input("areaElement", "The AreaElement to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("errorOverride", "Optional error message to override the default error message. Only the contents of this string will be returned as an error.")]
        [Output("pass", "True if the AreaElement is null.")]
        public static bool IIsNull(this IAreaElement element, string methodName = "Method", string errorOverride = "")
        {
            return IsNull(element as dynamic, "AreaElement", errorOverride);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void ErrorMessage(string methodName, string type)
        {
            Reflection.Compute.RecordError($"Cannot evaluate {methodName} because the {type} is null.");
        }

        /***************************************************/
        /**** Private Methods - Fallback                ****/
        /***************************************************/

        private static bool IsNull(this IAreaElement element, string methodName = "Method", string errorOverride = "")
        {
            if (element == null)
            {
                if (string.IsNullOrEmpty(errorOverride))
                    ErrorMessage(methodName, "AreaElement");
                else
                    Reflection.Compute.RecordError(errorOverride);
                return true;
            }

            return false;
        }

    }
}