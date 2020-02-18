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

using BH.oM.Structure.SurfaceProperties;
using BH.oM.Structure.SectionProperties;
using BH.oM.Reflection.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets any modifiers from a property as an array of doubles. The modifiers are used to scale one or more of the property constants for analysis. Constants are multiplied with the modifiers, hence a modifier value of 1 means no change. \n" + 
                     "The values returned are in the following order: f11, f12, f22, m11, m12, m22, v13, v23, mass, weight. Method returns null if no modifiers are found.")]
        [Input("property", "The property to extract modifiers from.")]
        [Output("modifiers", "Returns the modifier values of the property as a double array in the following order: f11, f12, f22, m11, m12, m22, v13, v23, mass, weight. Returns null if no modifiers are found.")]
        public static double[] Modifiers(this ISurfaceProperty property)
        {
            object modifersObj;

            if (property.CustomData.TryGetValue("Modifiers", out modifersObj))
            {
                return modifersObj as double[];
            }

            return null;
        }

        /***************************************************/

        [Description("Gets any modifiers from a section as an array of doubles. The modifiers are used to scale one or more of the section constants for analysis.  Constants are multiplied with the modifiers, hence a modifier value of 1 means no change. \n" + 
                     "The values returned are in the following order: area, iy, iz, j, asy, asz. Method returns null if no modifiers are found.")]
        [Input("property", "The SectionProperty to extract modifiers from.")]
        [Output("modifiers", "Returns the modifier values of the section as a double array in the following order: area, iy, iz, j, asy, asz. Returns null if no modifiers are found.")]
        public static double[] Modifiers(this ISectionProperty property)
        {
            object modifersObj;

            if (property.CustomData.TryGetValue("Modifiers", out modifersObj))
            {
                return modifersObj as double[];
            }

            return null;
        }

        /***************************************************/
    }
}

