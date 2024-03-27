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

using System;
using System.Collections.Generic;
using BH.oM.Base;
using Force.DeepCloner;

namespace BH.Engine.Base
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static T ShallowClone<T>(this T obj)
        {
            return (T)_ShallowClone(obj as dynamic);
        }

        /***************************************************/

        public static T ShallowClone<T>(this T obj, bool newGuid = false) where T : IBHoMObject
        {
            return (T)_ShallowClone(obj, newGuid);
        }

        /***************************************************/
        /**** Private Methods                            ****/
        /***************************************************/

        private static IBHoMObject _ShallowClone(this IBHoMObject bhomObject, bool newGuid = false)
        {
            IBHoMObject clone = DeepClonerExtensions.ShallowClone(bhomObject);

            if (bhomObject.CustomData != null)
                clone.CustomData = new Dictionary<string, object>(bhomObject.CustomData);
            else
                clone.CustomData = new Dictionary<string, object>();

            if (bhomObject.Tags != null)
                clone.Tags = new HashSet<string>(bhomObject.Tags);
            else
                clone.Tags = new HashSet<string>();

            if (bhomObject.Fragments != null && bhomObject.Fragments.Count > 0)
                clone.Fragments = new FragmentSet(bhomObject.Fragments);
            else
                clone.Fragments = new FragmentSet();

            if (newGuid)
                clone.BHoM_Guid = Guid.NewGuid();

            return clone;
        }

        /***************************************************/

        private static object _ShallowClone(this object obj)
        {
            return DeepClonerExtensions.ShallowClone(obj);
        }

        /***************************************************/
    }
}





