﻿/*
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


using BH.oM.Structure.SurfaceProperties;
using System.Collections.Generic;
using System;

namespace BH.Engine.Structure
{
    public class SurfacePropertyComparer : IEqualityComparer<ISurfaceProperty>
    {
        /***************************************************/
        /****           Public Methods                  ****/
        /***************************************************/

        public bool Equals(ISurfaceProperty property1, ISurfaceProperty property2)
        {
            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(property1, property2))
                return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(property1, null) || Object.ReferenceEquals(property2, null))
                return false;

            if (property1 is ConstantThickness && property2 is ConstantThickness)
            {
                ConstantThickness prop1 = (ConstantThickness)property1;
                ConstantThickness prop2 = (ConstantThickness)property2;

                if (prop1.Name == prop2.Name &&
                    prop1.Material.Equals(prop2.Material) &&
                    prop1.PanelType == prop2.PanelType &&
                    prop1.Thickness == prop2.Thickness)
                    return true;
                else
                    return false;
            }

            return false;
        }

        /***************************************************/

        public int GetHashCode(ISurfaceProperty obj)
        {
            //Check whether the object is null
            if (Object.ReferenceEquals(obj, null)) return 0;

            return obj.Name == null ? 0 : obj.Name.GetHashCode();
        }

        /***************************************************/
    }
}



