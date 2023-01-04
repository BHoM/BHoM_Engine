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

using System;
using System.Collections.Generic;

using System.Linq;
using BH.oM.Environment;

using BH.Engine.Geometry;
using BH.oM.Geometry;

using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns whether a list of points contains every point in the second list - order is not relevant. Done by measuring the square distance between points and finding those that are under the square distance tolerance.")]
        [Input("controlPoints", "A collection of BHoM Geometry Points as the control list.")]
        [Input("measurePoints", "A collection of BHoM Geometry Points as the measure list.")]
        [Input("squareTolerance", "The tolerance for how close points can be to be the same point. Points who's distance is greater than this tolerance are considered to be separate points.")]
        [Output("doPointsMatch", "True if all of the measurePoints are within the controlPoints list (independent of list order), false otherwise.")]
        public static bool PointsMatch(this List<Point> controlPoints, List<Point> measurePoints, double squareTolerance = BH.oM.Geometry.Tolerance.Distance)
        {
            if (controlPoints.Count != measurePoints.Count) return false;

            foreach (Point p in controlPoints)
            {
                Point ptInMeasure = measurePoints.Where(x => p.SquareDistance(x) <= squareTolerance).FirstOrDefault();
                if (ptInMeasure == null)
                    return false; //Point did not have a match
            }

            return true; //No points returned false before now
        }
    }
}



