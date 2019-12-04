/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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
using BH.Engine;
using BH.oM.Data.Collections;
using BH.oM.Diffing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BH.Engine.Serialiser;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.Reflection;
using BH.Engine.Diffing;

namespace BH.Engine
{
    public class HashFragmComparer<T> : IEqualityComparer<T> where T : IBHoMObject
    {
        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/
        public List<string> PropertiesToIgnore { get; set; } = null;

        public HashFragmComparer(List<string> propertiesToIgnore = null)
        {
            if (propertiesToIgnore == null)
                propertiesToIgnore = new List<string>() { "BHoM_Guid", "CustomData", "Fragments" };

            PropertiesToIgnore = propertiesToIgnore;
        }
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public bool Equals(T x, T y)
        {
            if (x.SHA256Hash(PropertiesToIgnore) == y.SHA256Hash(PropertiesToIgnore))
                return true;
            else
                return false;
        }

        /***************************************************/

        public int GetHashCode(T obj)
        {
            return obj.GetHashFragment().GetHashCode();
        }
    }
}
