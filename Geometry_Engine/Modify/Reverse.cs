/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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

using BH.Engine.Base;
using BH.oM.Geometry;
using System;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods - Interface                ****/
        /***************************************************/

        [Description("Reverses the provided geometry.")]
        public static T IReverse<T>(this T geom)
        {
            return Reverse(geom as dynamic);
        }

        /***************************************************/
        /**** Public Methods - Vectors                  ****/
        /***************************************************/

        [Description("Reverses the input vector by inverting the signs of its components.")]
        public static void Reverse(this Vector vector)
        {
            vector.X = -vector.X;
            vector.Y = -vector.Y;
            vector.Z = -vector.Z;
        }


        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        [Description("Reverses the input line by swapping its start and end points.")]
        public static void Reverse(this Line line)
        {
            var startCopy = line.Start;
            line.Start = line.End;
            line.End = startCopy;
        }

        /***************************************************/

        [Description("Reverses the input polyline by reversing the order of its control points.")]
        public static void Reverse(this Polyline polyLine)
        {
            polyLine.ControlPoints = polyLine.ControlPoints.Reverse<Point>().ToList();
        }


        /***************************************************/
        /**** Public Methods - Surfaces                 ****/
        /***************************************************/

        [Description("Reverses the input Planarsurface by reversing its boundary.")]
        public static PlanarSurface Reverse(this PlanarSurface surf)
        {

            if (!(surf.ExternalBoundary is Polyline) || !(surf.InternalBoundaries.Where(c => c is Polyline || c is Line).Count() != 0))
            {
                BH.Engine.Reflection.Compute.RecordError("The reverse for PlanarSurface currently works only for Planarsurfaces defined by Polylines and with no internal boundaries.");
                return null;
            }

            return new PlanarSurface(surf.ExternalBoundary.IReverse(), surf.InternalBoundaries.Select(b => b.IReverse()).ToList());
        }

        /***************************************************/
    }
}

