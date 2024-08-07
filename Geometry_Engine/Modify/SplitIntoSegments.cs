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

using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Splits a line in equal segments so that it does not exceed the split interval.")]
        [Input("line", "Line to be split in to segements if it exceeds the split interval.")]
        [Input("splitInterval", "Length that the segments should not exceed.")]
        [Input("graceLength", "If the total length of the wall is lower than this number it will not be segmented. The Line will be returned.")]
        [Output("segments", "Segments of input line as a list of Lines.")]
        public static List<Line> SplitIntoSegments(Line line, double splitInterval, double graceLength = 0.0)
        {

            List<Line> segments = new List<Line>();

            if (line.Length() < graceLength)
            {
                segments.Add(line);
                return segments;
            }

            double splitNum = ((line.Length() - (line.Length() % splitInterval)) / splitInterval) + 1;
            Vector deltaVec = (line.End - line.Start) / splitNum;

            for (int i = 0; i < splitNum; i++)
            {
                segments.Add(Create.Line(line.Start.Translate(i * deltaVec), line.Start.Translate((i + 1) * deltaVec)));
            }
            return segments;
        }

        [Description("Splits a line in equal segments with gaps between so that a segment does not exceed the split interval.")]
        [Input("line", "Line to be split in to segements if it exceeds the split interval.")]
        [Input("splitInterval", "Length that the segments should not exceed.")]
        [Input("gapLength", "Length of the gap between each segment.")]
        [Input("graceLength", "If the total length of the wall is lower than this number it will not be segmented. The whole line is returned.")]
        [Input("fractionalDigits", "The number of fractional digits to round the segment length to. An integer between 0 and 15 inclusive. Zero will round to nearest 5/0.")]
        [Output("segments", "Segments of input line, not exceeding the split interval.")]
        public static List<Line> SplitIntoSegments(Line line, double splitInterval, double gapLength = 0.0, double graceLength = 0.0, int fractionalDigits = -1)
        {
            List<Line> segments = new List<Line>();

            if (line.Length() < graceLength)
            {
                segments.Add(line);
                return segments;
            }

            double splitNum = ((line.Length() - (line.Length() % splitInterval)) / splitInterval) + 1;

            double segmentLength = (line.Length() - gapLength * (splitNum - 1)) / splitNum;

            if (!(fractionalDigits == -1))
                segmentLength = Math.Round(segmentLength * 2, fractionalDigits, MidpointRounding.AwayFromZero) / 2;

            Vector segmentVec = (line.End - line.Start).Normalise() * segmentLength;
            Vector gapVec = (line.End - line.Start).Normalise() * gapLength;

            for (int i = 0; i < splitNum; i++)
            {
                segments.Add(Create.Line(line.Start.Translate(i * segmentVec + i * gapVec), line.Start.Translate((i + 1) * segmentVec + i * gapVec)));
            }
            return segments;
        }
    }
}
