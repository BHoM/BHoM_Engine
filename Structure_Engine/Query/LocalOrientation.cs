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
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Get the Vector basis system descibring the local axis orientation of the Panel in the global coordinate system where the z-axis is the normal of the panel and the x and y axes are the directions of the local in-plane axes.")]
        [Input("panel", "The Panel to extract the local orientation from from.")]
        [Output("orienation", "The local orientation of the Panel as a vector Basis.")]
        public static Basis LocalOrientation(this Panel panel)
        {
            Vector normal = panel.IsNull() ? null : Spatial.Query.Normal(panel);

            return normal != null ? LocalOrientation(normal, panel.OrientationAngle) : null;
        }

        /***************************************************/

        [Description("Get the Vector basis system descibring the local axes orientation of the faces of the FEMesh in the global coordinate system where the z-axis is the normal of each face and the x and y axes are the directions of the local in-plane axes.")]
        [Input("mesh", "The FEMesh to extract the local orientations from from.")]
        [Output("orienations", "The local orientations of the mesh faces as a list of vector Bases.")]
        public static List<Basis> LocalOrientations(this FEMesh mesh)
        {
            return mesh.IsNull() ? null : mesh.Faces.Select(x => x.LocalOrientation(mesh)).ToList();
        }

        /***************************************************/

        [Description("Get the Vector basis system descibring the local axes orientation of a face of a FEMesh in the global coordinate system where the z-axis is the normal of each face and the x and y axes are the directions of the local in-plane axes.")]
        [Input("face", "The FEMeshFace to extract the local orientation from from.")]
        [Input("mesh", "The FEMesh to which the face belongs.")]
        [Output("orienation", "The local orientation of the FEMeshFace as a vector Basis.")]
        public static Basis LocalOrientation(this FEMeshFace face, FEMesh mesh)
        {
            return face.IsNull(mesh) ? null : LocalOrientation(face.Normal(mesh), face.OrientationAngle);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Get the Vector basis system descibring the local axis orientation from a normal vector and an orientation angle. Used by both Panels and FEMeshes.")]
        [Input("normal", "The normal vector of the element.")]
        [Input("orientationAngle", "The orientation angle of the element.", typeof(Angle))]
        [Output("orienation", "The local orientation of the element as a vector Basis.")]
        private static Basis LocalOrientation(this Vector normal, double orientationAngle)
        {
            Vector localX, localY;

            if (normal.IsParallel(Vector.XAxis) == 0)
            {
                //Normal not parallel to global X
                localX = Vector.XAxis.Project(new Plane { Normal = normal }).Normalise();
                localX = localX.Rotate(orientationAngle, normal);
                localY = normal.CrossProduct(localX);
            }
            else
            {
                //Normal is parallel to global x
                localY = Vector.YAxis.Project(new Plane { Normal = normal }).Normalise();
                localY = localY.Rotate(orientationAngle, normal);
                localX = localY.CrossProduct(normal);
            }

            return new Basis(localX, localY, normal);

        }

        /***************************************************/
    }
}



