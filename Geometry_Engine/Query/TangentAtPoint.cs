/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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

        public static Vector TangentAtPoint(this Arc curve, Point point, double tolerance = Tolerance.Distance)
        {
            double parameterAtPoint = curve.ParameterAtPoint(point, tolerance);
            return parameterAtPoint == -1 ? null : curve.TangentAtParameter(parameterAtPoint, tolerance);
        }

        /***************************************************/

        public static Vector TangentAtPoint(this Circle curve, Point point, double tolerance = Tolerance.Distance)
        {
            double parameterAtPoint = curve.ParameterAtPoint(point, tolerance);
            return parameterAtPoint == -1 ? null : curve.TangentAtParameter(parameterAtPoint, tolerance);
        }

        /***************************************************/

        public static Vector TangentAtPoint(this Line curve, Point point, double tolerance = Tolerance.Distance)
        {
            double parameterAtPoint = curve.ParameterAtPoint(point, tolerance);
            return parameterAtPoint == -1 ? null : curve.TangentAtParameter(parameterAtPoint, tolerance);
        }

        /***************************************************/

        public static Vector TangentAtPoint(this PolyCurve curve, Point point, double tolerance = Tolerance.Distance)
        {
            double parameterAtPoint = curve.ParameterAtPoint(point, tolerance);
            return parameterAtPoint == -1 ? null : curve.TangentAtParameter(parameterAtPoint, tolerance);
        }

        /***************************************************/

        public static Vector TangentAtPoint(this Polyline curve, Point point, double tolerance = Tolerance.Distance)
        {
            double parameterAtPoint = curve.ParameterAtPoint(point, tolerance);
            return parameterAtPoint == -1 ? null : curve.TangentAtParameter(parameterAtPoint, tolerance);
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static Vector ITangentAtPoint(this ICurve curve, Point point, double tolerance = Tolerance.Distance)
        {
            return Query.TangentAtPoint(curve as dynamic, point, tolerance);
        }


        /***************************************************/
        /**** Private Methods - Fallback                ****/
        /***************************************************/

        public static Vector TangentAtPoint(this ICurve curve, Point point, double tolerance = Tolerance.Distance)
        {
            Base.Compute.RecordError($"TangentAtPoint is not implemented for ICurves of type: {curve.GetType().Name}.");
            return null;
        }

        /***************************************************/
    }
}


