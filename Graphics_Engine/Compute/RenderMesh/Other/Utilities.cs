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

using BH.oM.Geometry;
using BH.oM.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BH.Engine.Geometry;
using BH.oM.Base;
using System.ComponentModel;

namespace BH.Engine.Graphics
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Private Methods - Utilities               ****/
        /***************************************************/

        private static Vector Scale(this Vector vector, double factor)
        {
            return new Vector { X = vector.X * factor, Y = vector.Y * factor, Z = vector.Z * factor };
        }

        private static Vector Product(this Vector a, double scalar)
        {
            return Modify.Scale(a, scalar);
        }

        private static bool IsStraight(this ICurve curve)
        {
            if (curve.IStartDir() != curve.IEndDir())
                return false;

            List<double> pointParams = Enumerable.Range(0, 10).Select(i => (double)((double)i / (double)10)).ToList();

            List<Point> pointsOnCurve = pointParams.Select(par => curve.IPointAtParameter(par)).ToList();

            Line line = BH.Engine.Geometry.Create.Line(curve.IStartPoint(), curve.IEndPoint());

            List<Point> pointsOnLine = pointParams.Select(par => line.IPointAtParameter(par)).ToList();

            if (pointsOnCurve.SequenceEqual(pointsOnCurve))
                return true;

            return false;
        }
    }
}
