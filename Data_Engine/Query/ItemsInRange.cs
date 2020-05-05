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

using System;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.Base;
using System.Threading.Tasks;
using BH.oM.Data.Collections;
using BH.oM.Geometry;

namespace BH.Engine.Data
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static IEnumerable<T> ItemsInRange<T>(this NTree<T> tree, NBound bounds)
        {
            return ItemsInRange<T>((ITree)tree, bounds);
        }
        

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/
        
        private static IEnumerable<T> ItemsInRange<T>(ITree tree, NBound bounds)
        {
            if (tree.Bounds.IsInRange(bounds))
            {
                if (tree is NTree<T>)
                    return (tree as NTree<T>).Items.SelectMany(x => ItemsInRange<T>(x, bounds)).ToArray();
                else
                    return new T[] { (tree as Leaf<T>).Item };
            }
            return new T[0];
        }

        /***************************************************/

    }
}
