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
using BH.oM.Base.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static Point PointAtLength(this Arc curve, double length)
        {
            return curve.PointAtParameter(length / curve.Length());
        }

        /***************************************************/

        public static Point PointAtLength(this Circle curve, double length)
        {
            double alfa = 2 * Math.PI * length / curve.Length();
            Vector refVector = 1 - Math.Abs(curve.Normal.DotProduct(Vector.XAxis)) > Tolerance.Angle ? Vector.XAxis : Vector.ZAxis;
            Vector localX = curve.Normal.CrossProduct(refVector).Normalise() * curve.Radius;
            return Create.Point(localX.Rotate(alfa, curve.Normal)) + curve.Centre;
        }

        /***************************************************/

        public static Point PointAtLength(this Line curve, double length)
        {
            return PointAtParameter(curve, length / curve.Length());
        }

        /***************************************************/

        public static Point PointAtLength(this PolyCurve curve, double length)
        {
            double parameter = length / curve.Length();
            return curve.PointAtParameter(parameter);
        }

        /***************************************************/

        public static Point PointAtLength(this Polyline curve, double length)
        {
            double parameter = length / curve.Length();
            return curve.PointAtParameter(parameter);
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static Point IPointAtLength(this ICurve curve, double length)
        {
            if (length > curve.ILength())
                throw new ArgumentOutOfRangeException("Length must be less than the length of the curve"); // Turn into warning

            return PointAtLength(curve as dynamic, length);
        }


        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static Point PointAtLength(this ICurve curve, double length)
        {
            Base.Compute.RecordError($"PointAtLength is not implemented for ICurves of type: {curve.GetType().Name}.");
            return null;
        }

        /***************************************************/
    }
}



