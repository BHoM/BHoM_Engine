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
using BH.oM.Geometry;
using BH.oM.Base;
using BH.Engine.Geometry;

namespace BH.Engine.Structure
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Calculates the orientation angle of an area element based on the normal of the element and a provided local X direction.\n" +
                     "The orientation angle is calculated as the angle between local x and a reference vector in the plane of the element, defined by the normal.\n" +
                     "This reference vector is the global X-vector projected to the plane of the element, except for the case were the normal is parallel to the global X-axis." +
                     "For this case the reference vector is instead the cross product of global Y and the normal, projected to the plane of the element.")]
        [Input("normal", "The normal of the element.")]
        [Input("localX", "Vector to treat as local x of the element. If this vector is not in the plane of the element it will get projected. If the vector is parallel to the normal of the element the operation will fail and a NaN value will be returned.")]
        [Output("orientationAngle", "The calculated orientation angle of the area element. Will return NaN if the normal and local x are parallel.")]
        public static double OrientationAngleAreaElement(this Vector normal, Vector localX)
        {
            if (localX == null || normal == null)
            {
                Base.Compute.RecordError("The provided normal and/or local x are null. The orientation angle could not be calculated.");
                return double.NaN;
            }

            localX = localX.Normalise();
            normal = normal.Normalise();
            double dot = normal.DotProduct(localX);

            if (Math.Abs(1 - dot) < Tolerance.Angle)
            {
                Base.Compute.RecordError("The provided local x is parallel to the normal of the element. The orientation angle could not be calculated.");
                return double.NaN;
            }
            else if (Math.Abs(dot) > Tolerance.Angle)
            {
                Base.Compute.RecordWarning("The provided local x is not in the plane of the element and will get projected");
                localX = localX.Project(new Plane { Normal = normal });
            }

            Vector refVec;

            if (normal.IsParallel(Vector.XAxis) == 0)
            {
                //Normal is not parallel to the global X-axis
                refVec = Vector.XAxis;
            }
            else
            {
                //Normal _is_ parallel to the global X-axis
                refVec = Vector.YAxis.CrossProduct(normal);
            }

            return refVec.Angle(localX, new Plane { Normal = normal });
        }

        /***************************************************/
    }
}




