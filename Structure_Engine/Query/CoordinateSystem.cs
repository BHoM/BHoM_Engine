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

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Structure.Elements;
using BH.oM.Geometry;
using BH.oM.Geometry.CoordinateSystem;
using BH.Engine.Geometry;
using BH.Engine.Spatial;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Get the carteseian coordinate system descibring the position and local orientation of the node in the global coordinate system.")]
        [Input("node", "The Node to extract the local coordinate system from.")]
        [Output("coordinateSystem", "The local cartesian coordinate system of the Node.")]
        public static Cartesian CoordinateSystem(this Node node)
        {
            return node.IsNull() ? null : Engine.Geometry.Create.CartesianCoordinateSystem(node.Position, node.Orientation.X, node.Orientation.Y);
        }

        /***************************************************/

        [Description("Get the carteseian coordinate system descibring the position and local orientation of the Bar in the global coordinate system where the Bar tangent is the local x-axis and the normal is the local z-axis.")]
        [Input("bar", "The Bar to extract the local coordinate system from.")]
        [Output("coordinateSystem", "The local cartesian coordinate system of the Bar.")]
        public static Cartesian CoordinateSystem(this Bar bar)
        {
            Vector tan = bar?.Tangent(true);
            Vector ax = bar?.Normal()?.CrossProduct(tan);
            return tan != null && ax != null ? Engine.Geometry.Create.CartesianCoordinateSystem(bar.StartNode.Position, tan, ax) : null;
        }

        /***************************************************/

        [Description("Get the Cartesian coordinate system describing the position and local orientation of the Panel in the global coordinate system where the z-axis is the normal of the Panel and the x and y axes are the directions of the local in-plane axes.")]
        [Input("panel", "The Panel to extract the local coordinate system from.")]
        [Output("coordinateSystem", "The local cartesian coordinate system of the Panel.")]
        public static Cartesian CoordinateSystem(this Panel panel)
        {
            Basis orientation = panel?.LocalOrientation();
            Point centroid;
            try
            {
                centroid = panel?.Centroid();
            }
            catch 
            {
                centroid = null;
            }

            if (orientation != null && centroid == null)
            {
                Base.Compute.RecordWarning("Panel Centroid could not be calculated. CoordinateSystem will use control point average as substitute.");
                centroid = panel.ControlPoints().Average();
            }

            return orientation != null ? new Cartesian(centroid, orientation.X, orientation.Y, orientation.Z) : null;
        }

        /***************************************************/

        [Description("Get the Cartesian coordinate system descibring the position and local orientation of the FEMeshFaces of the FEMesh in the global coordinate system where the z-axis is the normal of the FEMeshFace and the x and y axes are the directions of the local in-plane axes.")]
        [Input("mesh", "The FEMesh to extract the local coordinate systems of the FEMeshFaces from.")]
        [Output("coordinateSystems", "The local cartesian coordinate systems of the FEMeshFaces of the FEMesh.")]
        public static List<Cartesian> CoordinateSystem(this FEMesh mesh)
        {
            return mesh.IsNull() ? null : mesh.Faces.Select(x => x.CoordinateSystem(mesh)).ToList();
        }

        /***************************************************/

        [Description("Get the Cartesian coordinate system descibring the position and local orientation of the FEMeshFace in the global coordinate system where the z-axis is the normal of the FEMeshFace and the x and y axes are the directions of the local in-plane axes.")]
        [Input("face", "The FEMeshFace to extract the local coordinate system from.")]
        [Input("mesh", "The FEMesh to which the face belongs.")]
        [Output("coordinateSystem", "The local cartesian coordinate system of the FEMeshFace.")]
        public static Cartesian CoordinateSystem(this FEMeshFace face, FEMesh mesh)
        {
            Basis orientation = face?.LocalOrientation(mesh);
            return orientation != null ? new Cartesian(face.NodeListIndices.Select(i => mesh.Nodes[i].Position).Average(), orientation.X, orientation.Y, orientation.Z) : null;
        }

        /***************************************************/
    }
}




