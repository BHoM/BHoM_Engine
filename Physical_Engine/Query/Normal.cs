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

using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Physical.Elements;
using BH.oM.Base;

using BH.Engine.Base;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.oM.Physical.FramingProperties;
using System;

namespace BH.Engine.Physical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the normal of a FramingElement, which would be the Z-axis in the local coordinate syetem")]
        [Input("framingElement", "The FramingElement to evaluate the normal of")]
        [Output("normal", "The FramingElements normal vector")]
        public static Vector Normal(this IFramingElement framingElement)
        {
            if(framingElement == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the normal of a null framing element.");
                return null;
            }

            double orientationAngle = 0;
            if (!(framingElement.Property is ConstantFramingProperty))
                Base.Compute.RecordWarning("No ConstantFramingProperty found, OrientationAngle set as 0");
            else
                orientationAngle = (framingElement.Property as ConstantFramingProperty).OrientationAngle;

            return framingElement.Location.ElementNormal(orientationAngle);
        }

        /***************************************************/
    }
}




