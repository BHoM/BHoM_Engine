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
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Explicitly create a Mesh3D. Only checks validity of list lengths.")]
        [Input("vertices", "All the point objects which define the mesh.")]
        [Input("faces", "Faces containing indices pointing to the vertices which define them.")]
        [Input("cellRelation", "A parallel list to the faces detailing an indicative index for which cell they are connected to.")]
        [Output("mesh3d", "A volumetric mesh.")]
        public static Mesh3D Mesh3D(List<Point> vertices, List<Face> faces, List<CellRelation> cellRelation)
        {
            if (faces.Count != cellRelation.Count)
            {
                Engine.Base.Compute.RecordError("The number of cell relations must match the number of faces.");
                return null;
            }

            return new Mesh3D(vertices, faces, cellRelation);
        }

        /***************************************************/
    }
}




