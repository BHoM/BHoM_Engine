/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a polyline defined by the points which result in deviations from a straight line only. For example, if a polyline has 3 points in a straight line, the middle point is removed as part of this cleaning process. This is designed for closed polylines only")]
        [Input("polyline", "The polyline you wish to clean by removing unnecessary points")]
        [Input("tolerance", "The tolerance of the angle that defines a straight line. Default is set to the value defined by BH.oM.Geometry.Tolerance.Angle")]
        [Output("polyline", "The cleaned polyline")]
        public static Polyline CleanPolyline(this Polyline polyline, double angleTolerance = Tolerance.Angle, double distanceTolerance = Tolerance.Distance)
        {
            //This method is for closed polylines only at the moment
            if(!polyline.IsClosed())
            {
                BH.Engine.Reflection.Compute.RecordError("The CleanPolyline method is only for closed polylines and the input polyline is not closed.");
                return polyline;
            }

            List<Point> pnts = polyline.DiscontinuityPoints();

            if (pnts.Count < 3) return polyline; //If there's only two points here then this method isn't necessary

            int startIndex = 0;
            while (startIndex < pnts.Count)
            {
                Point first = pnts[startIndex];
                Point second = pnts[(startIndex + 1) % pnts.Count];
                Point third = pnts[(startIndex + 2) % pnts.Count];

                if (first.Angle(second, third) <= angleTolerance)
                {
                    //Delete the second point from the list, it's not necessary
                    pnts.RemoveAt((startIndex + 1) % pnts.Count);
                }
                else if (first.Distance(second) <= distanceTolerance)
                {
                    //Delete the second point as it is too close to the first to produce any meaningful change...
                    pnts.RemoveAt((startIndex + 1) % pnts.Count);
                }
                else
                    startIndex++; //Move onto the next point
            }

            if (pnts.First() != pnts.Last())
                pnts.Add(pnts.First()); //Reclose polyline

            Polyline pLine = new Polyline()
            {
                ControlPoints = pnts,
            };

            return pLine;
        }
    }
}
