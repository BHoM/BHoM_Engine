/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [PreviousVersion("8.1", "BH.Engine.Geometry.Create.PointGrid(BH.oM.Geometry.Point, BH.oM.Geometry.Vector, BH.oM.Geometry.Vector, System.Int32, System.Int32)")]
        [Description("Creates a two dimensional grid of points along the two provided vectors.")]
        [Input("start", "Base point of the grid.")]
        [Input("dir1", "First direction of the grid. Spacing in this direction will be determined by the length of the vector.")]
        [Input("dir2", "Second direction of the grid. Spacing in this direction will be determined by the length of the vector.")]
        [Input("nbPts1", "Number of points along the first direction.")]
        [Input("nbPts2", "Number of points along the second direction.")]
        [Output("grid", "The created grid of points as a nested list, where each inner list corresponds to all values along the first vector.")]
        public static List<List<Point>> PointGrid(Point start, Vector dir1, Vector dir2, int nbPts1, int nbPts2)
        {
            List<List<Point>> pts = new List<List<Point>>();
            for (int i = 0; i < nbPts1; i++)
            {
                List<Point> row = new List<Point>();
                for (int j = 0; j < nbPts2; j++)
                {
                    row.Add(start + i * dir1 + j * dir2);
                }
                pts.Add(row);
            }

            return pts;
        }

        /***************************************************/
    }
}
