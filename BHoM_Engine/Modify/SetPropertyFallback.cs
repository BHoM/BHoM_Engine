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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BH.oM.Base;
using BH.oM.Base.Attributes;

namespace BH.Engine.Base
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Sets a property of a custom object.")]
        public static IBHoMObject SetPropertyFallback(this IBHoMObject obj, string propertyName, object value, bool isSilent = false)
        {
            if (value is IFragment)
            {
                // Handle fragments
                obj.Fragments.Add(value as IFragment); 
            }
            else
            {
                // Otherwise add to custom data
                obj.CustomData[propertyName] = value;

                if (!isSilent)
                    Compute.RecordWarning($"{obj} does not contain any property with the name {propertyName}. The value is being set as custom data.");
            }
                
            return obj;
        }

        /***************************************************/
    }
}






