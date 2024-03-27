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

using BH.oM.Data.Collections;
using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Convert
    {
        /*********************************************/
        /**** Public  Methods                     ****/
        /*********************************************/

        [Description("Converts a Mesh3D into a regular mesh, which removes all the information about the volumetric cells.")]
        [Input("mesh3d", "Volumetric 3d mesh to convert.")]
        [Output("mesh", "A regular mesh with the same vertices and faces as the Mesh3D.")]
        public static Mesh ToMesh(this Mesh3D mesh3d)
        {
            return new Mesh()
            {
                Vertices = mesh3d.Vertices.ToList(),
                Faces = mesh3d.Faces.ToList(),
            };
        }

        /*********************************************/

    }
}



