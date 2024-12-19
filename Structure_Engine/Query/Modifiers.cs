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

using BH.oM.Structure.SurfaceProperties;
using BH.oM.Structure.SectionProperties;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;
using BH.oM.Structure.Fragments;
using BH.Engine.Base;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets any modifiers from a property as an array of doubles. The modifiers are used to scale one or more of the property constants for analysis. Constants are multiplied with the modifiers, hence a modifier value of 1 means no change. \n" +
                     "The values returned are in the following order: FXX, FXY, FYY, MXX, MXY, MYY, VXZ, VYZ, Mass, Weight. Method returns null if no modifiers are found.")]
        [Input("property", "The property to extract modifiers from.")]
        [Output("modifiers", "Returns the modifier values of the property as a double array in the following order: FXX, FXY, FYY, MXX, MXY, MYY, VXZ, VYZ, Mass, Weight. Returns null if no modifiers are found.")]
        public static double[] Modifiers(this ISurfaceProperty property)
        {
            if (property.IsNull())
                return null;

            SurfacePropertyModifier modifier = property.FindFragment<SurfacePropertyModifier>();

            if (modifier == null)
                return null;

            return new double[] { modifier.FXX, modifier.FXY, modifier.FYY, modifier.MXX, modifier.MXY, modifier.MYY, modifier.VXZ, modifier.VYZ, modifier.Mass, modifier.Weight };
        }

        /***************************************************/

        [Description("Gets any modifiers from a section as an array of doubles. The modifiers are used to scale one or more of the section constants for analysis.  Constants are multiplied with the modifiers, hence a modifier value of 1 means no change. \n" +
                     "The values returned are in the following order: Area, Iy, Iz, J, Asy, Asz. Method returns null if no modifiers are found.")]
        [Input("property", "The SectionProperty to extract modifiers from.")]
        [Output("modifiers", "Returns the modifier values of the section as a double array in the following order: Area, Iy, Iz, J, Asy, Asz. Returns null if no modifiers are found.")]
        public static double[] Modifiers(this ISectionProperty property)
        {
            if (property.IsNull())
                return null;

            SectionModifier modifier = property.FindFragment<SectionModifier>();

            if (modifier == null)
                return null;

            return new double[] { modifier.Area, modifier.Iy, modifier.Iz, modifier.J, modifier.Asy, modifier.Asz };
        }

        /***************************************************/
    }
}






