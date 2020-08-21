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

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
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

        [Description("Calculates the orientation angle of a Bar based on a normal vector and a centreline.")]
        [Input("normal", "Vector to be used as normal of the Bar. This vector should generally be orthogonal to the Bar, if it is not, it will be made orthogonal by projecting it to the section plane of the Bar (a plane that has that Bar tangent as its normal). This means that the Normal cannot be paralell to the Tangent of the Bar. \n" +
                         "Vector will be used to determain the orientation angle of the Bar. This is done by measuring the counter clockwise angle in the section plane of the Bar between a reference Vector and the provided Vector. For a non-vertical Bar, the reference vector will be the global Z-axis. For a vertical bar the reference vector will be a vector that is orthogonal to the tangent vector of the Bar and the global Y-axis.")]
        [Input("centreline", "Geometrical Line, centreline of the Bar.")]
        [Output("orientationAngle", "The calculated orientation angle of the bar based on the provided geometry and normal. Will return NaN if the orientation can not be calculated.")]
        public static double OrientationAngleBar(this Vector normal, Line centreline)
        {
            double orientationAngle;

            if (normal == null)
                orientationAngle = 0;
            else
            {
                normal = normal.Normalise();
                Vector tan = (centreline.End - centreline.Start).Normalise();

                double dot = normal.DotProduct(tan);

                if (Math.Abs(1 - dot) < Tolerance.Angle)
                {
                    Reflection.Compute.RecordError("The normal is parallell to the centreline of the Bar. Orientation angle could not be calcualted.");
                    return double.NaN;
                }
                else if (Math.Abs(dot) > Tolerance.Angle)
                {
                    Reflection.Compute.RecordWarning("Normal is not othogonal to the centreline and will get projected.");
                }

                Vector reference;

                if (!centreline.IsVertical())
                    reference = Vector.ZAxis;
                else
                {
                    reference = tan.CrossProduct(Vector.YAxis);
                }

                orientationAngle = reference.Angle(normal, new Plane { Normal = tan });
            }

            return orientationAngle;
        }

        /***************************************************/

        [Description("Calculates the orientation angle of an area element based on the normal of the element and a provided local X direction.\n" +
                     "The orientation angle is calculated as the angle between localX and a reference vector in the plane of the element, defined by the normal.\n" +
                     "This reference vector is the global X-vector projected to the plane of the element, except for the case were the normal is parallel to the global X-axis." +
                     "For this case the reference vector is instead the crossproduct of global Y and the normal, projected to the plane of the element.")]
        [Input("normal", "The normal of the element.")]
        [Input("localX", "Vector to treat as local x of the element. If this vector is not in the plane of the element it will get projected. If the vector is parallel to the normal of the element the operation will fail and a NaN value will be returned.")]
        [Output("orientationAngle", "The calculated orientation angle of the area element. Will return NaN if the normal and local x are parallel.")]
        public static double OrientationAngleAreaElement(this Vector normal, Vector localX)
        {
            localX = localX.Normalise();
            normal = normal.Normalise();
            double dot = normal.DotProduct(localX);

            if (Math.Abs(1 - dot) < Tolerance.Angle)
            {
                Reflection.Compute.RecordError("The provided localX is parallel to the normal of the element. The orientation angle could not be calculated.");
                return double.NaN;
            }
            else if (Math.Abs(dot) > Tolerance.Angle)
            {
                Reflection.Compute.RecordWarning("The provided localX is not in the Plane of the element and will get projected");
                localX = localX.Project(new Plane { Normal = normal });
            }

            Vector refVec;

            if (normal.IsParallel(Vector.XAxis) == 0)
            {
                //Normal is not paralell to the global X-axis
                refVec = Vector.XAxis;
            }
            else
            {
                //Normal _is_ paralell to the global X-axis
                refVec = Vector.YAxis.CrossProduct(normal);
            }

            return refVec.Angle(localX, new Plane { Normal = normal });
        }

        /***************************************************/
    }
}
