/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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

using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using BH.oM.Spatial.ShapeProfiles;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Spatial
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a kite profile based on input dimensions. Method generates edge curves based on the inputs.")]
        [InputFromProperty("width1")]
        [InputFromProperty("angle1")]
        [InputFromProperty("thickness")]
        [Output("kite", "The created KiteProfile.")]
        public static KiteProfile KiteProfile(double width1, double angle1, double thickness)
        {
            if (!AreFinite(width1, angle1, thickness))
                return null;

            if (!ArePositive(width1, angle1, thickness))
                return null;

            if ((width1 * Math.Sin(angle1 / 2) / Math.Sqrt(2)) / (Math.Sin(Math.PI * 0.75 - (angle1 / 2))) <= thickness)
            {
                InvalidRatioError(nameof(thickness), $"{nameof(width1)} and {nameof(angle1)}");
                return null;
            }

            double angle2 = Math.PI - angle1;
            double tolerance = 1e-3;
            if (angle2 < tolerance || angle2 > Math.PI - tolerance)
            {
                Base.Compute.RecordError("Angles must be well between 0 and Pi");
                return null;
            }

            List<ICurve> curves = KiteProfileCurves(width1, angle1, thickness);
            return new KiteProfile(width1, angle1, thickness, curves);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<ICurve> KiteProfileCurves(double width1, double angle1, double thickness)
        {
            //Kite symmetric about the x-axis with the corner of angle1 at the origin, right angles at the side corners, and the
            //last corner on the positive x-axis. The void is the kite offset inwards by the thickness.
            double cos = Math.Cos(angle1 / 2);
            double sin = Math.Sin(angle1 / 2);

            //Outer loop counter-clockwise
            double ax = width1 * cos;
            double ay = width1 * sin;
            List<ProfileCorner> outer = new List<ProfileCorner> { new ProfileCorner(0, 0, 0), new ProfileCorner(ax, -ay, 0), new ProfileCorner(width1 / cos, 0, 0), new ProfileCorner(ax, ay, 0) };

            //Inner loop clockwise
            double px = thickness / sin;
            double innerLength = width1 - thickness - thickness * cos / sin;
            double cx = px + innerLength * cos;
            double cy = innerLength * sin;
            List<ProfileCorner> inner = new List<ProfileCorner> { new ProfileCorner(px, 0, 0), new ProfileCorner(cx, cy, 0), new ProfileCorner((width1 - thickness) / cos, 0, 0), new ProfileCorner(cx, -cy, 0) };

            return ProfileCurves(true, outer, inner);
        }

        /***************************************************/
    }
}






