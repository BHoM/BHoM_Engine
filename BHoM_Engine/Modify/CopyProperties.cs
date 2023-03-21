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

using System;
using System.Reflection;
using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Base
{
    public static partial class Modify
    {
        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/

        [Description("Copy values from parameters of a source object to parameters of the same names in the target object.")]
        [Input("source", "The source object containing parameter values to copy from.")]
        [Input("target", "The target object containing parameter values to copy to.")]
        public static void CopyIdenticalProperties(this object sourceObj, object targetObj)
        {
            if (sourceObj == null || targetObj == null)
            {
                Compute.RecordWarning("Can't copy parameter values to or from a null object.");
                return;
            }

            Type targetType = targetObj.GetType();
            Type sourceType = sourceObj.GetType();

            foreach (PropertyInfo sourcePropInfo in sourceType.GetProperties())
            {
                if (!sourcePropInfo.CanRead)
                    continue;

                PropertyInfo targetPropInfo = targetType.GetProperty(sourcePropInfo.Name);
                if (targetPropInfo == null)
                    continue;

                if (targetPropInfo.CanWrite == false)
                    continue;

                if (targetPropInfo.GetSetMethod(true)?.IsPrivate == true)
                    continue;

                if (targetPropInfo.PropertyType.IsAssignableFrom(sourcePropInfo.PropertyType) == false)
                    continue;

                targetPropInfo.SetValue(targetObj, sourcePropInfo.GetValue(sourceObj, null), null);
            }
        }

        /***************************************************/
    }
}




