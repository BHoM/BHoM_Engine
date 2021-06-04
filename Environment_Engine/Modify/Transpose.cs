/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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

using BH.oM.Reflection.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods - Arrays                   ****/
        /***************************************************/

        [Description("Transpose a rectangular matrix")]
        [Input("matrix", "A 2D matrix")]
        [Output("matrix", "A 2D matrix")]
        public static double[,] Transpose(this double[,] matrix)
        {
            int h = matrix.GetLength(0);
            int w = matrix.GetLength(1);
            double[,] transpose = new double[w, h];

            for (int m = 0; m < h; m++)
            {
                for (int n = 0; n < w; n++)
                    transpose[m, n] = matrix[n, m];
            }

            return transpose;
        }

        [Description("Transpose a rectangular matrix")]
        [Input("matrix", "A 2D matrix")]
        [Output("matrix", "A 2D matrix")]
        public static int[,] Transpose(this int[,] matrix)
        {
            int h = matrix.GetLength(0);
            int w = matrix.GetLength(1);
            int[,] transpose = new int[w, h];

            for (int m = 0; m < h; m++)
            {
                for (int n = 0; n < w; n++)
                    transpose[m, n] = matrix[n, m];
            }

            return transpose;
        }

        [Description("Transpose a rectangular matrix")]
        [Input("matrix", "A 2D matrix")]
        [Output("matrix", "A 2D matrix")]
        public static bool[,] Transpose(this bool[,] matrix)
        {
            int h = matrix.GetLength(0);
            int w = matrix.GetLength(1);
            bool[,] transpose = new bool[w, h];

            for (int m = 0; m < h; m++)
            {
                for (int n = 0; n < w; n++)
                    transpose[m, n] = matrix[n, m];
            }

            return transpose;
        }

        [Description("Transpose a rectangular 2D array")]
        [Input("array2D", "A rectangular 2D array")]
        [Output("array2D", "A rectangular 2D array")]
        public static List<List<T>> Transpose<T>(this List<List<T>> array2D)
        {
            var longest = array2D.Any() ? array2D.Max(l => l.Count) : 0;
            List<List<T>> outer = new List<List<T>>(longest);
            for (int i = 0; i < longest; i++)
            {
                outer.Add(new List<T>(array2D.Count));
            }

            for (int j = 0; j < array2D.Count; j++)
            {
                for (int i = 0; i < longest; i++)
                {
                    outer[i].Add(array2D[j].Count > i ? array2D[j][i] : default(T));
                }
            }

            return outer;
        }
    }
}

