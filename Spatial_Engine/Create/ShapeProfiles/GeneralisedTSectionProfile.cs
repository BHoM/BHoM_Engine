/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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
using BH.oM.Base.Attributes;
using BH.Engine.Geometry;
using System.ComponentModel;

namespace BH.Engine.Spatial
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a T-shaped profile based on input dimensions. Method generates edge curves based on the inputs.")]
        [InputFromProperty("height")]
        [InputFromProperty("webThickness")]
        [InputFromProperty("leftOutstandWidth")]
        [InputFromProperty("leftOutstandThickness")]
        [InputFromProperty("rightOutstandWidth")]
        [InputFromProperty("rightOutstandThickness")]
        [InputFromProperty("mirrorAboutLocalY")]
        [Output("genT", "The created GeneralisedTSectionProfile.")]
        public static GeneralisedTSectionProfile GeneralisedTSectionProfile(double height, double webThickness, double leftOutstandWidth, double leftOutstandThickness, double rightOutstandWidth, double rightOutstandThickness, bool mirrorAboutLocalY = false)
        {
            if (height < leftOutstandThickness)
            {
                InvalidRatioError("height", "leftOutstandThickness");
                return null;
            }

            if (height < rightOutstandThickness)
            {
                InvalidRatioError("height", "rightOutstandThickness");
                return null;
            }

            if (leftOutstandThickness <= 0 && leftOutstandWidth > 0 || leftOutstandWidth <= 0 && leftOutstandThickness > 0)
            {
                InvalidRatioError("leftOutstandThickness", "leftOutstandWidth");
                return null;
            }

            if (rightOutstandThickness <= 0 && rightOutstandWidth > 0 || rightOutstandWidth <= 0 && rightOutstandThickness > 0)
            {
                InvalidRatioError("rightOutstandThickness", "rightOutstandWidth");
                return null;
            }

            if (height <= 0 || webThickness <= 0 || leftOutstandThickness < 0 || leftOutstandWidth < 0 || rightOutstandThickness < 0 || rightOutstandWidth < 0)
            {
                Engine.Base.Compute.RecordError("Input length less or equal to 0");
                return null;
            }

            List<ICurve> curves = GeneralisedTeeProfileCurves(height, webThickness, leftOutstandWidth, leftOutstandThickness, rightOutstandWidth, rightOutstandThickness);

            if (mirrorAboutLocalY)
                curves = curves.MirrorAboutLocalY();

            return new GeneralisedTSectionProfile(height, webThickness, leftOutstandWidth, leftOutstandThickness, rightOutstandWidth, rightOutstandThickness, mirrorAboutLocalY, curves);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<ICurve> GeneralisedTeeProfileCurves(double height, double webThickness, double leftOutstandWidth, double leftOutstandThickness, double rightOutstandWidth, double rightOutstandThickness)
        {
            List<ICurve> perimeter = new List<ICurve>();
            Point p = new Point { X = -webThickness / 2, Y = 0, Z = 0 };

            Vector xAxis = oM.Geometry.Vector.XAxis;
            Vector yAxis = oM.Geometry.Vector.YAxis;
            Point origin = oM.Geometry.Point.Origin;

            perimeter.Add(new Line { Start = p, End = p = p + yAxis * (height - leftOutstandThickness) });
            perimeter.Add(new Line { Start = p, End = p = p + xAxis * (-leftOutstandWidth) });
            perimeter.Add(new Line { Start = p, End = p = p + yAxis * (leftOutstandThickness) });
            perimeter.Add(new Line { Start = p, End = p = p + xAxis * (leftOutstandWidth + webThickness + rightOutstandWidth) });
            perimeter.Add(new Line { Start = p, End = p = p + yAxis * (-rightOutstandThickness) });
            perimeter.Add(new Line { Start = p, End = p = p + xAxis * (-rightOutstandWidth) });
            perimeter.Add(new Line { Start = p, End = p = p + yAxis * (-height + rightOutstandThickness) });
            perimeter.Add(new Line { Start = p, End = p = p + xAxis * (-webThickness) });

            Point centroid = perimeter.IJoin().Centroid();
            Vector translation = Point.Origin - centroid;

            return perimeter.Select(x => x.ITranslate(translation)).ToList();
        }

        /***************************************************/

    }
}





