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

using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using BH.oM.Spatial.ShapeProfiles;
using BH.oM.Geometry;
using System;
using BH.Engine.Reflection;
using BH.oM.Reflection.Attributes;
using BH.Engine.Geometry;
using System.ComponentModel;

namespace BH.Engine.Spatial
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a C-shaped profile based on input dimensions. Method generates edge curves based on the inputs.")]
        [InputFromProperty("height")]
        [InputFromProperty("flangeWidth")]
        [InputFromProperty("webThickness")]
        [InputFromProperty("flangeThickness")]
        [InputFromProperty("flangeSlope")]
        [InputFromProperty("rootRadius")]
        [InputFromProperty("toeRadius")]
        [InputFromProperty("mirrorAboutLocalZ")]
        [Output("channel", "The created ChannelProfile.")]
        public static TaperFlangeChannelProfile TaperFlangeChannelProfile(double height, double flangeWidth, double webThickness, double flangeThickness, double flangeSlope, double rootRadius = 0, double toeRadius = 0, bool mirrorAboutLocalZ = false)
        {
            if (height < flangeThickness * 2 + rootRadius * 2 || height <= flangeThickness * 2)
            {
                InvalidRatioError("height", "flangeThickness and rootRadius");
                return null;
            }

            if (flangeWidth < webThickness + rootRadius + toeRadius)
            {
                InvalidRatioError("width", "webthickness, toeRadius and rootRadius");
                return null;
            }

            if (flangeSlope < 0)
            {
                Reflection.Compute.RecordError("Flange slope must be positive. Suggest approximately 0.16 radians");
                return null;
            }

            if (flangeSlope > Math.Atan(2 * flangeThickness / flangeWidth))
            {
                InvalidRatioError("Width", "FlangeThickness and FlangeSlope");
                return null;
            }

            if (toeRadius > flangeThickness - flangeWidth/2 * Math.Tan(flangeSlope) )
            {
                InvalidRatioError("flangeThickness", "toeRadius");
                return null;
            }

            if (height <= 0 || flangeWidth <= 0 || webThickness <= 0 || flangeThickness <= 0 || rootRadius < 0 || toeRadius < 0)
            {
                Engine.Reflection.Compute.RecordError("Input length less or equal to 0");
                return null;
            }

            List<ICurve> curves = TaperFlangeChannelProfileCurves(height, flangeWidth, webThickness, flangeThickness, flangeSlope, rootRadius, toeRadius);

            if (mirrorAboutLocalZ)
                curves = curves.MirrorAboutLocalZ();

            return new TaperFlangeChannelProfile(height, flangeWidth, webThickness, flangeThickness, flangeSlope, rootRadius, toeRadius, mirrorAboutLocalZ, curves);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<ICurve> TaperFlangeChannelProfileCurves(double height, double width, double wt, double ft, double slope, double r1, double r2)
        {
            List<ICurve> perimeter = new List<ICurve>();
            Point p = Point.Origin;

            Vector xAxis = oM.Geometry.Vector.XAxis;
            Vector yAxis = oM.Geometry.Vector.YAxis;

            Line l0 = new Line { Start = p, End = p = p + xAxis * width };
            Line l1 = new Line { Start = p, End = p = p + yAxis * (ft - width / 2 * Math.Tan(slope)) };
            Line l2 = new Line { Start = p, End = p = p - xAxis * (width - wt) + yAxis * (width - wt) * Math.Tan(slope) };
            Line l3 = new Line { Start = p, End = p = p + yAxis * (height - 2*(ft + (width / 2 - wt) * Math.Tan(slope))) };
            Line l4 = new Line { Start = p, End = p = p + xAxis * (width - wt) + yAxis * (width - wt) * Math.Tan(slope) };
            Line l5 = new Line { Start = p, End = p = p + yAxis * (ft - width / 2 * Math.Tan(slope)) };
            Line l6 = new Line { Start = p, End = p = p - xAxis * width };
            Line l7 = new Line { Start = p, End = p = p - yAxis * height };

            perimeter.Add(l0);
            List<ICurve> fillet = Fillet(l1, l2, r2);
            perimeter.AddRange(fillet.GetRange(0, fillet.Count - 1));
            fillet = Fillet(fillet.Last() as Line, l3, r1);
            perimeter.AddRange(fillet.GetRange(0, fillet.Count - 1));
            fillet = Fillet(fillet.Last() as Line, l4, r1);
            perimeter.AddRange(fillet.GetRange(0, fillet.Count - 1));
            fillet = Fillet(fillet.Last() as Line, l5, r2);
            perimeter.AddRange(fillet);
            perimeter.Add(l6);
            perimeter.Add(l7);

            return perimeter;
        }

        /***************************************************/

    }
}

