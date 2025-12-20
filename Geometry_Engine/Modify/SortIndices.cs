/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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

using BH.Engine.Base;
using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Sorts the indices of mesh faces to ensure consistent ordering. For each face, rotates the vertex indices so the smallest index comes first, then sorts all faces by their vertex indices.")]
        [Input("mesh", "The mesh whose face indices should be sorted.")]
        public static void SortIndices(this Mesh mesh)
        {
            for (int i = 0; i < mesh.Faces.Count; i++)
            {
                Face f = mesh.Faces[i];

                List<int> indices = new List<int> { f.A, f.B, f.C };
                if (f.D != -1)
                    indices.Add(f.D);

                int min = indices.Min();
                int j = indices.IndexOf(min);

                indices = indices.ShiftList(j);
                f.A = indices[0];
                f.B = indices[1];
                f.C = indices[2];

                if (f.D != -1)
                    f.D = indices[3];
            }

            mesh.Faces = mesh.Faces.OrderBy(x => x.A).ThenBy(x => x.B).ThenBy(x => x.C).ThenBy(x => x.D).ToList();
        }

        /***************************************************/
    }
}

