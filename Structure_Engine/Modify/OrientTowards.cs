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
using BH.oM.Base;
using BH.Engine.Geometry;
using BH.Engine.Spatial;
using BH.Engine.Base;

namespace BH.Engine.Structure
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Orients the normal of the Bar towards the provided point. The normal is calculated as the vector from the centre of the bar to the provided point.")]
        [Input("bar", "The Bar to update the orientation of.")]
        [Input("orientationPoint", "The point to orient the normal of the Bar towards.")]
        [Output("bar", "Bar with updated orientation. If the calcualtion of the orientation angle fails, the unmodified bar is returned.")]
        public static Bar OrientTowards(this Bar bar, Point orientationPoint)
        {
            if (bar.IsNull() || orientationPoint.IsNull())
                return null;

            Point centre = bar.Centreline().ClosestPoint(orientationPoint);
            Vector normal = (orientationPoint - centre).Normalise();

            return bar.SetNormal(normal);
        }

        /***************************************************/

        [Description("Orients the direction of the local x of the Panel towards the provided point. The local x is calculated as the vector from the centroid of the Panel to the provided point.")]
        [Input("panel", "The Panel to update the local orientation of.")]
        [Input("orientationPoint", "The point to orient the local x of the Panel towards.")]
        [Output("panel", "Panel with updated local orientation. If the calcualtion of the orientation angle fails, the unmodified Panel is returned.")]
        public static Panel OrientTowards(this Panel panel, Point orientationPoint)
        {
            if (panel.IsNull() || orientationPoint.IsNull())
                return null;

            Point centre = panel.Centroid();
            Vector localX = (orientationPoint - centre).Normalise();

            return panel.SetLocalOrientation(localX);
        }

        /***************************************************/

        [Description("Orients the direction of the local x of each FEMeshFace of the FEMesh towards the provided point. The local x vectors are calculated as the vector from the centroid of each FEMeshFace of the FEMesh to the provided point.")]
        [Input("mesh", "The FEMesh to update the local orientations of.")]
        [Input("orientationPoint", "The point to orient the local x vectors of the FEMeshFaces of the FEMesh towards.")]
        [Output("mesh", "FEMesh with updated local orientations. If the calcualtion of the orientation angle fails for an FEMeshFace, it remains unchanged.")]
        public static FEMesh OrientTowards(this FEMesh mesh, Point orientationPoint)
        {
            if (mesh.IsNull() || orientationPoint.IsNull())
                return null;

            FEMesh clone = mesh.ShallowClone();
            clone.Faces = clone.Faces.Select(x => x.OrientTowards(mesh, orientationPoint)).ToList();
            return clone;
        }

        /***************************************************/

        [Description("Orients the direction of the local x of the FEMeshFace towards the provided point. The local x vector is calculated as the vector from the centroid of the FEMeshFace of to the provided point.")]
        [Input("face", "The FEMeshFace to update the local orientation of.")]
        [Input("mesh", "The FEMesh to which the face belongs.")]
        [Input("orientationPoint", "The point to orient the local x of the FEMeshFaces towards.")]
        [Output("meshFace", "FEMeshFace with updated local orientation. If the calcualtion of the orientation angle fails, the unmodified FEMeshFace is returned.")]
        public static FEMeshFace OrientTowards(this FEMeshFace face, FEMesh mesh, Point orientationPoint)
        {
            if (face.IsNull() || mesh.IsNull() || orientationPoint.IsNull())
                return null;

            Point centre = face.NodeListIndices.Select(i => mesh.Nodes[i].Position).Average();
            Vector localX = (orientationPoint - centre).Normalise();
            return face.SetLocalOrientation(mesh, localX);
        }

        /***************************************************/
    }
}



