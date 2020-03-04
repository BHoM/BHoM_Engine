/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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

using BH.oM.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Interface Methods                         ****/
        /***************************************************/

        public static object IItem(this object obj, int index)
        {
            return Item(obj as dynamic, index);
        }

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static object Item<T>(this List<T> list, int index)
        {
            return list[index];
        }

        /***************************************************/

        public static object Item<T>(this Output<T> output, int index)
        {
            if (index == 0)
                return output.Item1;
            else
                return null;
        }

        /*************************************/

        public static object Item<T1, T2>(this Output<T1, T2> output, int index)
        {
            switch (index)
            {
                case 0:
                    return output.Item1;
                case 1:
                    return output.Item2;
                default:
                    return null;
            }
        }

        /*************************************/

        public static object Item<T1, T2, T3>(this Output<T1, T2, T3> output, int index)
        {
            switch (index)
            {
                case 0:
                    return output.Item1;
                case 1:
                    return output.Item2;
                case 2:
                    return output.Item3;
                default:
                    return null;
            }
        }

        /*************************************/

        public static object Item<T1, T2, T3, T4>(this Output<T1, T2, T3, T4> output, int index)
        {
            switch (index)
            {
                case 0:
                    return output.Item1;
                case 1:
                    return output.Item2;
                case 2:
                    return output.Item3;
                case 3:
                    return output.Item4;
                default:
                    return null;
            }
        }

        /*************************************/

        public static object Item<T1, T2, T3, T4, T5>(this Output<T1, T2, T3, T4, T5> output, int index)
        {
            switch (index)
            {
                case 0:
                    return output.Item1;
                case 1:
                    return output.Item2;
                case 2:
                    return output.Item3;
                case 3:
                    return output.Item4;
                case 4:
                    return output.Item5;
                default:
                    return null;
            }
        }

        /***************************************************/

        public static int Item<T>(this object obj)
        {
            return 0;
        }

        /***************************************************/

        public static int Item<T>(this List<T> list)
        {
            return list.Count;
        }
    }
}

