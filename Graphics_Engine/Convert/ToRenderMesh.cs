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
using BH.oM.Graphics;
using BH.oM.Base.Attributes;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Graphics
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods - Graphics                 ****/
        /***************************************************/

        [Description("Converts a Geometrical Mesh to a RenderMesh.")]
        [Input("mesh", "Mesh to be converted to RenderMesh.")]
        [Output("renderMesh", "RenderMesh converted from the input mesh.")]
        public static RenderMesh ToRenderMesh(this Mesh mesh)
        {
            if(mesh == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot convert a null mesh to a render mesh.");
                return null;
            }

            return new RenderMesh { Vertices = mesh.Vertices.Select(x => (RenderPoint)x).ToList(), Faces = mesh.Faces };
        }

        /***************************************************/
    }
}




