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

using BH.oM.Base;
using BH.oM.Base.Attributes;
using System;
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

        [Description("Gets the precomputed integer Binomal coefficient (n k).")]
        [Input("n", "Upper term of the binomial cooeficient. Should be larger or equal to 0 and larger than k. A value of 0 is returned if this is not the case.")]
        [Input("k", "Lower term of the binomial cooeficient. Should be larger or equal to 0 and smaller than n. A value of 0 is returned if this is not the case.")]
        [Output("bin", "The binomal cooeficient relating to (n k)")]
        public static int Binomal(int n, int k)
        {
            if (k < 0) //k needs to be larger or equal to 0. For negative values 0 is returned
                return 0;

            if (n < k)  //k is larger than or equal 0, so only need to ensure that n is larger than or equal to k as this implicitly checks that n is larger or equal to 0 as well
                return 0;

            lock (m_binomalsLock)
            {
                int i = m_binomals.Count;

                while (i <= n)
                {
                    //Compute the binomal as triangular jagged array as Pascals triangle
                    //https://en.wikipedia.org/wiki/Binomial_coefficient

                    int[] row = new int[i + 1]; //length as 1 more than current row (due to zero indexing) First row has 1 item, 2nd has 2 etc.
                    row[0] = 1;         //Edge value always 1

                    for (int j = 1; j < i; j++)
                    {
                        row[j] = m_binomals[i - 1][j - 1] + m_binomals[i - 1][j];     //Non edge values sum of the two values "above"
                    }
                    row[i] = 1;         //Edge value always 1
                    m_binomals.Add(row);
                    i++;
                }
            }
            return m_binomals[n][k];
        }


        /***************************************************/

        private static List<int[]> m_binomals = new List<int[]>();
        private static object m_binomalsLock = new object();

        /***************************************************/
    }
}
