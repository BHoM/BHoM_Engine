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


using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.Engine.Geometry;
using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using BH.oM.Base.Attributes;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the bars local z-axis, generally the major axis direction of the section of the Bar. \n" +
                     "For non - vertical members the local z-axis is aligned with the global Z-axis and rotated with the orientation angle around the local x-axis. \n" +
                     "For vertical members the local y-axis is aligned with the global Y-axis and rotated with the orientation angle around the local x-axis. For this case the normal will be the vector orthogonal to the local x-axis and local y-axis.")]
        [Input("bar", "The Bar to evaluate the normal of.")]
        [Output("normal", "Vector representing the local z-axis of the Bar.")]
        public static Vector Normal(this Bar bar)
        {
            return bar.IsNull() ? null : bar.Centreline().ElementNormal(bar.OrientationAngle);
        }

        /***************************************************/

        [Description("Returns the local z-axes of all FEMeshFaces in the FEMesh. Can only extract normals for 3 or 4-sided faces.")]
        [Input("mesh", "The FEMesh to extract the face normals from.")]
        [Output("normal", "List of vectors representing the local z-axes of mesh faces. List order corresponds to the order of the faces.")]
        public static List<Vector> Normals(this FEMesh mesh)
        {
            return mesh.IsNull() ? null : mesh.Faces.Select(x => x.Normal(mesh)).ToList();
        }

        /***************************************************/

        [Description("Returns the local z-axis of an FEMeshFace. Can only extract normals for 3 or 4-sided faces.")]
        [Input("face", "The FEMeshFace to evaluate the normal of.")]
        [Input("mesh", "The FEMesh to which the face belongs.")]
        [Output("normal", "Vector representing the local z-axis of a mesh face.")]
        public static Vector Normal(this FEMeshFace face, FEMesh mesh)
        {
            if (face.IsNull(mesh))
                return null;

            if (face.NodeListIndices.Count < 3)
            {
                Engine.Base.Compute.RecordError("Face has insufficient number of nodes to calculate normal.");
                return null;
            }
            else if (face.NodeListIndices.Count > 4)
            {
                Engine.Base.Compute.RecordError("Can only determine normal from 3 or 4 sided faces.");
                return null;
            }

            Point pA = mesh.Nodes[face.NodeListIndices[0]].Position;
            Point pB = mesh.Nodes[face.NodeListIndices[1]].Position;
            Point pC = mesh.Nodes[face.NodeListIndices[2]].Position;

            Vector normal;
            if (face.NodeListIndices.Count == 3)
                normal = Engine.Geometry.Query.CrossProduct(pB - pA, pC - pB);
            else
            {
                Point pD = mesh.Nodes[face.NodeListIndices[3]].Position;
                normal = (Engine.Geometry.Query.CrossProduct(pA - pD, pB - pA)) + (Engine.Geometry.Query.CrossProduct(pC - pB, pD - pC));
            }

            return normal.Normalise();
        }

        /***************************************************/
        /**** Public Methods - Interface methods        ****/
        /***************************************************/

        [Description("Returns the local z-axis of the IAreaElement.")]
        [Input("areaElement", "The element to evaluate the normal of.")]
        [Output("normal", "Vector representing the local z-axis element.")]
        public static Vector INormal(this IAreaElement areaElement)
        {
            return areaElement.IsNull() ? null : Normal(areaElement as dynamic);
        }

        /***************************************************/
        /**** Private Methods - fall back               ****/
        /***************************************************/

        private static Vector Normal(this IAreaElement areaElement)
        {
            Base.Compute.RecordWarning("Cannot get normal for element of type " + areaElement.GetType().Name);
            return null;
        }

        /***************************************************/
        /**** Public Methods - ToBeRemoved               ****/
        /***************************************************/

        [ToBeRemoved("3.1", "ToBeRemoved by method targeting IElement2D.")]
        [Description("Returns the Panels local z-axis, a vector orthogonal to the plane of the Panel. This is found by fitting a plane through all the edge curves and taking the Normal from this plane.")]
        [Input("panel", "The Panel to evaluate the normal of.")]
        [Output("normal", "Vector representing the local z-axis Panel.")]
        public static Vector Normal(this Panel panel)
        {
            return Engine.Spatial.Query.Normal(panel);
        }

        /***************************************************/

    }
}



