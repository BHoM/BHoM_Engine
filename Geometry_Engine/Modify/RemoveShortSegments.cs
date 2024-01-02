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

using System.Collections.Generic;
using System.Linq;

using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a polyline that has short segments removed. For example, if the polyline has 2 points less than the minimumSegmentLength apart, then only one point will be retained and the short segment will be merged with the next segment")]
        [Input("polyline", "The polyline you wish to clean by removing short segments")]
        [Input("minimumSegmentLength", "The tolerance of what a short segment is. Segments greater than this length will be kept, segments shorter will be cleaned (removed). Default is set to the value defined by BH.oM.Geometry.Tolerance.Distance")]
        [Output("polyline", "The cleaned polyline")]
        public static Polyline RemoveShortSegments(this Polyline polyline, double minimumSegmentLength = Tolerance.Distance, double distanceTolerance = Tolerance.Distance)
        {            
            List<Point> pnts = new List<Point>(polyline.IControlPoints());
            Point originalLastPoint = pnts.Last();

            int startIndex = 0;
            int maxIndex = polyline.IsClosed(distanceTolerance) ? pnts.Count : pnts.Count - 1;
            while (startIndex < maxIndex)
            {
                Point first = pnts[startIndex];
                Point second = pnts[(startIndex + 1) % pnts.Count];                

                if (first.Distance(second) <= minimumSegmentLength)
                {
                    pnts.RemoveAt((startIndex + 1) % pnts.Count); //Delete the second point as it is too close to the first to produce any meaningful change...
                    maxIndex--;
                }
                else
                    startIndex++; //Move onto the next point
            }
            
            if (!polyline.IsClosed(distanceTolerance))
            {
                pnts.Remove(pnts.Last());
                pnts.Add(originalLastPoint);                
            }
            else //Only re-close if original polyline is closed
            {
                pnts.Add(pnts.First());                
            }

            while (pnts.Last().Distance(pnts[pnts.Count() - 2]) < minimumSegmentLength)
            {
                pnts.RemoveAt(pnts.Count - 2);
            }

            Polyline pLine = new Polyline()
            {
                ControlPoints = pnts,
            };

            return pLine;
        }
        /***************************************************/
    }
}





