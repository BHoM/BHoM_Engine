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
using BH.Engine.Base;
using BH.Engine.Geometry;

namespace BH.Engine.Structure
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Sets the direction of the local x vector of the Panel. This is done by calculation of a new orientation angle matching the provided vector.")]
        [Input("panel", "The Panel to set the local orientation to.")]
        [Input("localX", "Vector to set as local x of the panel. If this vector is not in the plane of the Panel it will get projected. If the vector is parallel to the normal of the Panel the operation will fail and the Panel will not be updated.")]
        [Output("panel", "The Panel with updated orientation.")]
        public static Panel SetLocalOrientation(this Panel panel, Vector localX)
        {
            if (panel.IsNull() || localX.IsNull())
                return null;

            Panel clone = panel.ShallowClone();
            Vector normal = Spatial.Query.Normal(panel);

            double orientationAngle = Compute.OrientationAngleAreaElement(normal, localX);

            if (!double.IsNaN(orientationAngle))
                clone.OrientationAngle = orientationAngle;

            return clone;
        }

        /***************************************************/

        [Description("Sets the direction of the local x vector of all FEMeshFaces of the FEMesh. This is done by calculation of a new orientation angles matching the provided vector.")]
        [Input("mesh", "The FEMesh to set the local orientations to.")]
        [Input("localX", "Vector to set as local x of the FEMeshFaces of the FEMesh. If this vector is not in the plane of the FEMeshFace it will get projected. If the vector is parallel to the normal of the FEMeshFace the operation will fail and the FEMeshFace will not be updated.")]
        [Output("mesh", "The FEMesh with updated face orientations.")]
        public static FEMesh SetLocalOrientations(this FEMesh mesh, Vector localX)
        {
            if (mesh.IsNull() || localX.IsNull())
                return null;

            FEMesh clone = mesh.ShallowClone();
            clone.Faces = clone.Faces.Select(x => x.SetLocalOrientation(mesh, localX)).ToList();
            return clone;
        }

        /***************************************************/

        [Description("Sets the direction of the local x vector of the FEMeshFace. This is done by calculation of a new orientation angle matching the provided vector.")]
        [Input("face", "The FEMeshFace to set the local orientation to.")]
        [Input("mesh", "The FEMesh to which the face belongs.")]
        [Input("localX", "Vector to set as local x of the FEMeshFaces. If this vector is not in the plane of the FEMeshFace it will get projected. If the vector is parallel to the normal of the FEMeshFace the operation will fail and the FEMeshFace will not be updated.")]
        [Output("face", "The FEMeshFace with updated face orientation.")]
        public static FEMeshFace SetLocalOrientation(this FEMeshFace face, FEMesh mesh, Vector localX)
        {
            if (face.IsNull() || mesh.IsNull() || localX.IsNull())
                return null;

            FEMeshFace clone = face.ShallowClone();
            Vector normal = face.Normal(mesh);

            double orientationAngle = Compute.OrientationAngleAreaElement(normal, localX);

            if (!double.IsNaN(orientationAngle))
                clone.OrientationAngle = orientationAngle;

            return clone;
        }

        /***************************************************/
    }
}



