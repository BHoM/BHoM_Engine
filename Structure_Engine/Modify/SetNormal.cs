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

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Structure.Elements;
using BH.oM.Geometry;
using BH.Engine.Base;
using BH.Engine.Geometry;
using BH.Engine.Spatial;

namespace BH.Engine.Structure
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Aligns the normal of the Bar with the provided normal by updating the OrientationAngle of the former.")]
        [Input("bar", "The Bar to update.")]
        [Input("normal", "Vector to be used as normal of the Bar. This vector should generally be orthogonal to the Bar, if it is not, it will be made orthogonal by projecting it to the section plane of the Bar (a plane that has that Bar tangent as its normal). This means that the Normal cannot be parallel to the Tangent of the Bar. \n" +
                         "Vector will be used to determine the orientation angle of the Bar. This is done by measuring the counter clockwise angle in the section plane of the Bar between a reference Vector and the provided Vector. For a non-vertical Bar, the reference vector will be the global Z-axis. For a vertical bar the reference vector will be a vector that is orthogonal to the tangent vector of the Bar and the global Y-axis.")]
        [Output("bar", "Bar with updated orientation angle. If the orientation angle could not be calculated, the unchanged bar is returned.")]
        public static Bar SetNormal(this Bar bar, Vector normal)
        {
            if (bar.IsNull() || normal.IsNull())
                return null;

            double orientationAngle = normal.OrientationAngleLinear(bar.Centreline());

            //Could not calculate the orientation angle. Return the original bar
            if (double.IsNaN(orientationAngle))
                return bar;

            Bar clone = bar.ShallowClone();

            clone.OrientationAngle = orientationAngle;

            return clone;
        }

        /***************************************************/
    }
}




