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

using BH.oM.Structure.SurfaceProperties;
using BH.oM.Structure.SectionProperties;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Checks if a SurfaceProperty has any modifiers by first checking if any modifiers has been assigned, and if any of them are set to a value different than 1.")]
        [Input("property", "The property to check for modifiers.")]
        [Output("result", "Returns true if any modifiers exists on the section.")]
        public static bool HasModifiers(this ISurfaceProperty property)
        {
            if (property.IsNull())
                return false;

            double[] modifiers = property.Modifiers();

            if (modifiers == null)
                return false;

            foreach (double modifier in modifiers)
            {
                if (modifier != 1) return true;
            }
            return false;
        }

        /***************************************************/

        [Description("Checks if a SectionProperty has any modifiers by first checking if any modifiers has been assigned, and if any of them are set to a value different than 1.")]
        [Input("property", "The property to check for modifiers.")]
        [Output("result", "Returns true if any modifiers exists on the section.")]
        public static bool HasModifiers(this ISectionProperty property)
        {
            if (property.IsNull())
                return false;

            double[] modifiers = property.Modifiers();

            if (modifiers == null)
                return false;

            foreach (double modifier in modifiers)
            {
                if (modifier != 1) return true;
            }
            return false;
        }

        /***************************************************/
    }
}





