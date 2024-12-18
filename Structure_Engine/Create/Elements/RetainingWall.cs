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
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.oM.Structure.SurfaceProperties;
using BH.Engine.Base;
using BH.Engine.Geometry;
using BH.Engine.Spatial;
using BH.Engine.Structure;
using BH.oM.Quantities.Attributes;


namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a RetainingWall from a Stem, Footing and properties.")]
        [Input("stem", "The Stem of the RetainingWall.")]
        [Input("footing", "The Footing of the RetainingWall.")]
        [Input("retainedHeight", "The retained height of soil measured from the bottom of the wall Footing.", typeof(Length))]
        [Input("coverDepth", "The distance from top of Footing to finished floor level on the exposed face.", typeof(Length))]
        [Input("retentionAngle", "A property of the material being retained measured from the horizontal plane.", typeof(Angle))]
        [Input("groundWaterDepth", "The distance from the base of the Footing to ground water level.", typeof(Length))]
        [Output("retainingWall", "The created RetainingWall containing the stem and footing.")]
        public static RetainingWall RetainingWall(Stem stem, PadFoundation footing, double retainedHeight, double coverDepth, double retentionAngle, double groundWaterDepth = 0)
        {
            if (stem.IsNull() || footing.IsNull())
                return null;

            return Query.IsValid(stem, footing) ? new RetainingWall()
            {
                Stem = stem,
                Footing = footing,
                RetainedHeight = retainedHeight,
                CoverDepth = coverDepth,
                RetentionAngle = retentionAngle,
                GroundWaterDepth = groundWaterDepth
            }
            : null;
        }

        /***************************************************/

        [Description("Create RetainingWall from a Line and defining properties.")]
        [Input("line", "A Line parallell to the XY plane to build the RetainingWall from.")]
        [Input("retainedHeight", "The retained height of soil measured from the bottom of the wall Footing.", typeof(Length))]
        [Input("normal", "Normal to the surface of the stem denoting the direction of the retained face.", typeof(Vector))]
        [Input("stemThickness", "Thickness at the top and bottom of the stem.", typeof(Length))]
        [Input("toeLength", "Length of the toe of the footing.", typeof(Length))]
        [Input("heelLength", "Length of the heel of the footing.", typeof(Length))]
        [Input("footingThickness", "Thickness of the footing.", typeof(Length))]
        [Input("coverDepth", "The distance from top of Footing to finished floor level on the exposed face.", typeof(Length))]
        [Input("retentionAngle", "A property of the material being retained measured from the horizontal plane.", typeof(Angle))]
        [Output("retainingWall", "RetainingWall with specified properties.")]
        public static RetainingWall RetainingWall(Line line, double retainedHeight, Vector normal, double stemThickness, double toeLength,
            double heelLength, ConstantThickness footingThickness, double coverDepth, double retentionAngle)
        {
            if (line.Start == null && line.End == null || line.IsNull())
                return null;

            if (!line.IsInPlane(Geometry.Create.Plane(line.Start, Vector.ZAxis)))
            {
                Base.Compute.RecordError("Provided line is not parallel to the XY plane. Please provide a line parallel to the XY plane.");
                return null;
            }

            PolyCurve stemOutline = new PolyCurve();
            PolyCurve footingOutline = new PolyCurve();

            Vector unitNormal = normal.Normalise();

            //Create the footing outline. 
            Line toeLine = line.Translate(unitNormal * (toeLength + stemThickness / 2));
            Line heelLine = line.Translate(unitNormal * (heelLength + stemThickness / 2) * -1).Reverse();

            footingOutline.Curves = new List<ICurve> { toeLine, Geometry.Create.Line(toeLine.End, heelLine.Start), heelLine, Geometry.Create.Line(heelLine.End, toeLine.Start) };

            //Create the Stem outline.
            Line topLine = line.Translate(Vector.ZAxis * retainedHeight).Reverse();

            stemOutline.Curves = new List<ICurve> { line, Geometry.Create.Line(line.End, topLine.Start), topLine, Geometry.Create.Line(topLine.End, line.Start) };

            return RetainingWall(Create.Stem(stemOutline, stemThickness, unitNormal), Create.PadFoundation(footingOutline, footingThickness), retainedHeight, coverDepth, retentionAngle);
        }

        /***************************************************/
    }

}