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

using BH.oM.Base.Attributes;
using System.ComponentModel;
using BH.oM.Physical.Elements;
using BH.oM.Geometry;
using BH.Engine.Base;

namespace BH.Engine.Physical
{
    public static partial class Modify
    {

        /***************************************************/

        [Description("Replaces the location curve of the IFramingElement with the provided curve.")]
        [Input("framingElement", "The framingElement to modify the location curve of.")]
        [Input("curve", "The new location of the IFramingElement as a ICurve.")]
        [Output("element1D", "The IFramingElement with modified location curve.")]
        public static IFramingElement SetGeometry(this IFramingElement framingElement, ICurve curve)
        {
            if (framingElement == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot set the geometry of a null framing element.");
                return null;
            }

            IFramingElement clone = framingElement.ShallowClone();
            clone.Location = curve.DeepClone();
            return clone;
        }

        /***************************************************/

    }
}





