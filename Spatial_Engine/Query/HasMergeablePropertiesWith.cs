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

using BH.oM.Dimensional;
using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Spatial
{
    public static partial class Query
    {
        /******************************************/
        /****        Public Methods            ****/
        /******************************************/

        [Description("Evaluates if the two elements non-geometrical data is equal to the point that they could be merged into one object.")]
        [Input("element", "An IElement0D to compare the properties of with an other IElement0D.")]
        [Input("other", "The IElement0D to compare with the other IElement0D.")]
        [Output("equal", "True if the Objects non-geometrical property is equal to the point that they could be merged into one object.")]
        public static bool IHasMergeablePropertiesWith(this IElement0D element, IElement0D other)
        {
            return HasMergeablePropertiesWithIElement(element, other);
        }

        /******************************************/

        [Description("Evaluates if the two elements non-geometrical data is equal to the point that they could be merged into one object.")]
        [Input("element", "An IElement1D to compare the properties of with an other IElement1D.")]
        [Input("other", "The IElement1D to compare with the other IElement1D.")]
        [Output("equal", "True if the Objects non-geometrical property is equal to the point that they could be merged into one object.")]
        public static bool IHasMergeablePropertiesWith(this IElement1D element, IElement1D other)
        {
            return HasMergeablePropertiesWithIElement(element, other);
        }

        /******************************************/

        [Description("Evaluates if the two elements non-geometrical data is equal to the point that they could be merged into one object.")]
        [Input("element", "An IElement2D to compare the properties of with an other IElement2D.")]
        [Input("other", "The IElement2D to compare with the other IElement2D.")]
        [Output("equal", "True if the Objects non-geometrical property is equal to the point that they could be merged into one object.")]
        public static bool IHasMergeablePropertiesWith(this IElement2D element, IElement2D other)
        {
            return HasMergeablePropertiesWithIElement(element, other);
        }


        /******************************************/
        /****        Private Methods           ****/
        /******************************************/

        private static bool HasMergeablePropertiesWithIElement(this IElement element, IElement other)
        {
            // Geometrical objects don't have properties
            if (typeof(IGeometry).IsAssignableFrom(element.GetType()) && typeof(IGeometry).IsAssignableFrom(other.GetType()))
                return true;

            // Different types are different
            if (element.GetType() != other.GetType())
                return false;

            // look for a specific comparing method
            object result = Base.Compute.RunExtensionMethod(element, "HasMergeablePropertiesWith", new object[] { other });

            if (result == null || !result.GetType().IsAssignableFrom(typeof(bool)))
            {
                Base.Compute.RecordWarning("No comparer found for comparing the properties");
                return false;
            }

            return (bool)result;
        }

        /******************************************/
    }
}






