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
using BH.oM.Geometry;
using BH.oM.Physical.Reinforcement.BS8666;
using BH.oM.Base.Attributes;

namespace BH.Engine.Physical
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a ShapeCode object using the parameters provided. Refer to the object descriptions for alignment.")]
        [Output("shapeCode", "A ShapeCode to be used with Reinforcement objects.")]
        public static ShapeCode26 ShapeCode26(double a, double c, double d, double e, double diameter, double bendRadius)
        {
            if (a < Tolerance.Distance || c < Tolerance.Distance || d < Tolerance.Distance || e < Tolerance.Distance)
            {
                Base.Compute.RecordError("One or more of the parameters given is zero and therefore the ShapeCode cannot be created.");
                return null;
            }

            double dRed = d - diameter;  //Height centreline
            double alpha = Math.Atan(e / dRed);  //Angle of the upper corner of the triangle F-d-B 
            double beta = alpha / 2 + Math.PI / 4;      //Angle of the bisector between A and B
            double arcAngle = Math.PI / 2 - alpha;      //Angle of the arc

            double r = bendRadius + diameter / 2;   //Centreline bend radius
            double x = r / Math.Tan(beta);                              //Distance from AB corner with 0 radius to end of A
            double s = Math.Sqrt(r * r + x * x);                        //Distance from arc centre to AB corner
            double t = s - r;                                           //Hypotenous of triangle from AB corner to arc
            double yRed = r / s * t;                                  //Reduction in the leng reduction. This is X-distance from arc centre to AB using the fact that triangles are of the same shape

            dRed = dRed - 2 * yRed;

            double b = Math.Pow(Math.Pow(dRed, 2) + Math.Pow(e, 2), 0.5);

            ShapeCode26 shapeCode = new ShapeCode26(a, b, c, d, e, diameter, bendRadius);

            return shapeCode.IIsCompliant() ? shapeCode : null;
        }

        /***************************************************/
    }
}



