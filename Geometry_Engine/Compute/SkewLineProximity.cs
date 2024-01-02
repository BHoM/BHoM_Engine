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
using BH.oM.Geometry;
using BH.oM.Base;
using System.Collections.Generic;
using BH.oM.Base.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Output<double, double> SkewLineProximity(this Line line1, Line line2, double angleTolerance = Tolerance.Angle)
        {
            Vector v1 = line1.End - line1.Start;
            Vector v2 = line2.End - line2.Start;
            Vector v1N = v1.Normalise();
            Vector v2N = v2.Normalise();

            if (v1N == null || v2N == null || 1 - Math.Abs(v1N.DotProduct(v2N)) <= angleTolerance)
                return null;

            Point p1 = line1.Start;
            Point p2 = line2.Start;

            Vector cp = v1.CrossProduct(v2);
            Vector n1 = v1.CrossProduct(-cp);
            Vector n2 = v2.CrossProduct(cp);

            double t1 = (p2 - p1) * n2 / (v1 * n2);
            double t2 = (p1 - p2) * n1 / (v2 * n1);

            return new Output<double, double> { Item1 = t1, Item2 = t2 };
        }

        /***************************************************/
    }
}




