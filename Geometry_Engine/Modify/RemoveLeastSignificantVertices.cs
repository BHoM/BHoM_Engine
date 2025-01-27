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

        [Description("Returns a polyline defined by the points which result in deviations from a straight line only. For example, if a polyline has 3 points in a straight line, the middle point is removed as part of this cleaning process")]
        [Input("polyline", "The polyline you wish to clean by removing unnecessary points")]
        [Input("smallestAcceptableAngle", "The tolerance of the angle that defines a straight line. Default is set to the value defined by BH.oM.Geometry.Tolerance.Angle")]
        [Output("polyline", "The cleaned polyline")]        
        public static Polyline RemoveLeastSignificantVertices(this Polyline polyline, double smallestAcceptableAngle = Tolerance.Angle, double angleTolerance = Tolerance.Angle, double distanceTolerance = Tolerance.Distance)
        {           
            List<Point> pnts = polyline.DiscontinuityPoints(distanceTolerance, angleTolerance);
            Point originalLastPoint = pnts.Last();

            int startIndex = 0;
            int maxIndex = polyline.IsClosed(distanceTolerance) ? pnts.Count : pnts.Count - 2;
            while (startIndex < maxIndex)
            {
                Point first = pnts[startIndex];
                Point second = pnts[(startIndex + 1) % pnts.Count];
                Point third = pnts[(startIndex + 2) % pnts.Count];

                if (first.Angle(second, third) <= smallestAcceptableAngle)
                {
                    pnts.RemoveAt((startIndex + 1) % pnts.Count);  //Delete the second point from the list, it's not necessary
                    maxIndex--;
                }
                else
                    startIndex++; //Move onto the next point
            }

            if (polyline.IsClosed(distanceTolerance)) //Only re-close if original polyline is closed           
            {
                pnts.Add(pnts.First());
            }

            Polyline pLine = new Polyline()
            {
                ControlPoints = pnts,
            };

            List<double> angles = new List<double>();
            foreach (BH.oM.Geometry.Point point in pnts)
            {
                double angle = point.Angle(pnts[(pnts.IndexOf(point) + 1) % pnts.Count], pnts[(pnts.IndexOf(point) + 2) % pnts.Count]);
                angles.Add(angle);
            }
            foreach (double angle in angles)
            {
                if (angle < smallestAcceptableAngle)
                {
                    BH.Engine.Base.Compute.RecordWarning("One ore more of the angles of the new polyline is smaller than smallestAcceptableAngle, choose a smaller value for smallestAcceptableAngle or proceed with the created polyline as it is.");
                }
            }
            
            return pLine;
        }
        /***************************************************/
    }    
}








