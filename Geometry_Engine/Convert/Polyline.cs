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

using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;

namespace BH.Engine.Geometry
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public  Methods - Curves                  ****/
        /***************************************************/

        public static Polyline ToPolyline(this PolyCurve curve)
        {
            if (curve.Curves.Count == 0)
                return new Polyline();

            List<Point> controlPoints = new List<Point> { curve.Curves[0].IStartPoint() };
            foreach (ICurve c in curve.SubParts())
            {
                if (c is Line)
                    controlPoints.Add((c as Line).End);
                else
                    return null;
            }

            return new Polyline { ControlPoints = controlPoints };
        }

        /***************************************************/

        public static Polyline ToPolyline(Line curve)
        {
            return new Polyline { ControlPoints = new List<Point> { curve.Start, curve.End } };
        }

        /***************************************************/

        public static Polyline ToPolyline(Polyline curve)
        {
            return curve;
        }


        /***************************************************/
        /**** Public Methods - Interface                ****/
        /***************************************************/

        public static Polyline IToPolyline(ICurve curve)
        {
            return ToPolyline(curve as dynamic);
        }


        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static Polyline ToPolyline(ICurve curve)
        {
            Base.Compute.RecordError($"ToPolyline is not implemented for ICurves of type: {curve.GetType().Name}.");
            return null;
        }

        /***************************************************/
    }
}


