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
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Physical.Elements;

using BH.Engine.Base;
using BH.oM.Physical.FramingProperties;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.Engine.Spatial;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Physical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the average cross-section area of a IFramingElementProperty in such a way that multiplying with the length of the element would give the volume")]
        [Input("framingProperty", "The framingProperty to evaluate the average area of")]
        [Output("averageArea", "The average cross-section area of a IFramingElementProperty", typeof(Area))]
        public static double IAverageProfileArea(this IFramingElementProperty framingProperty)
        {
            if(framingProperty == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the average profile area of a null framing property.");
                return 0;
            }

            return AverageProfileArea(framingProperty as dynamic);
        }

        /***************************************************/

        [Description("Gets the average cross-section area of a ConstantFramingProperty in such a way that multiplying with the length of the element would give the volume")]
        [Input("framingProperty", "The framingProperty to evaluate the average area of")]
        [Output("averageArea", "The average cross-section area of a ConstantFramingProperty", typeof(Area))]
        public static double AverageProfileArea(this ConstantFramingProperty framingProperty)
        {
            if (framingProperty == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the average profile area of a null constant framing property.");
                return 0;
            }

            if (framingProperty.Profile == null)
            {
                Engine.Base.Compute.RecordError("The framingProperty Average Profile Area could not be calculated as no Profile has been assigned. Returning zero Area.");
                return 0;
            }

            return framingProperty.Profile.Area();
        }


        /***************************************************/
        /****    private fallback method            ********/
        /***************************************************/

        private static double AverageProfileArea(this IFramingElementProperty framingProperty)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

    }
}





