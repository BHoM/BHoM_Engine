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

        [Description("Gets the faces which defines each cell in the mesh3d.")]
        [Input("mesh3d", "The Mesh3D to query the cells from.")]
        [Output("cells", "The cells of the Mesh3D defined as a list of faces for each cell.")]
        public static List<List<Face>> Cells(this Mesh3D mesh3d)
        {
            Dictionary<int, List<Face>> result = new Dictionary<int, List<Face>>();

            for (int i = 0; i < mesh3d.CellRelation.Count; i++)
            {
                int key = mesh3d.CellRelation[i].FromCell;

                if (!result.ContainsKey(key))
                    result[key] = new List<Face>();
                result[key].Add(mesh3d.Faces[i]);

                key = mesh3d.CellRelation[i].ToCell;

                if (!result.ContainsKey(key))
                    result[key] = new List<Face>();
                result[key].Add(mesh3d.Faces[i]);

            }

            if (result.ContainsKey(-1))
                result.Remove(-1);

            return result.Select(x => x.Value).ToList();
        }

        /***************************************************/
    }
}





