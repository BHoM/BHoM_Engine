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
using BH.oM.Reflection.Interface;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Interface Methods                         ****/
        /***************************************************/

        public static int ICount<T>(this List<T> list)
        {
            return Item(list as dynamic);
        }

        /***************************************************/

        public static int Count(this IOutput output)
        {
            return OutputCount(output as dynamic);
        }


        /***************************************************/
        /**** Public Methods                            ****/
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

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static int OutputCount<T>(this Output<T> output)
        {
            return 1;
        }

        /***************************************************/

        private static int OutputCount<T1,T2>(this Output<T1,T2> output)
        {
            return 2;
        }

        /***************************************************/

        private static object OutputCount<T1, T2, T3>(this Output<T1, T2, T3> output)
        {
            return 3;
        }

        /*************************************/

        private static object OutputCount<T1, T2, T3, T4>(this Output<T1, T2, T3, T4> output)
        {
            return 4;
        }

        /*************************************/

        private static object OutputCount<T1, T2, T3, T4, T5>(this Output<T1, T2, T3, T4, T5> output)
        {
            return 5;
        }

        /***************************************************/
        /**** Private Methods - fallback                ****/
        /***************************************************/

        private static object OutputCount(this object output)
        {
            return 0;
        }

        /*************************************/

    }
}

