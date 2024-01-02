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

using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Query the Mesh which only consists of the external faces of the Mesh3D.")]
        [Input("mesh3d", "The Mesh3D to query for its external face mesh.")]
        [Output("mesh", "A mesh consistent of all the faces in the Mesh3D which were only connected to one cell.")]
        public static Mesh ExternalFaceMesh(this Mesh3D mesh3d)
        {
            List<Face> externalFaces = new List<Face>();

            for (int i = 0; i < mesh3d.CellRelation.Count; i++)
            {
                CellRelation c = mesh3d.CellRelation[i];
                if (c.ToCell == -1 ^ c.FromCell == -1)
                {
                    externalFaces.Add(mesh3d.Faces[i]);
                }
            }
            
            return new Mesh()
            {
                Vertices = mesh3d.Vertices.ToList(), // Should only take the sub set of points, but that will mess with the indecies of the faces
                Faces = externalFaces,
            };
        }

        /***************************************************/

    }
}





