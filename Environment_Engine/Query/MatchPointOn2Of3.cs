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

using BH.oM.Environment.Elements;
using BH.oM.Geometry;
using System.Collections.Generic;
using System.Linq;

using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/

        [Description("Returns whether two points match on 2 out of 3 axes")]
        [Input("point", "A BHoM Geometry Point to compare against")]
        [Input("comparePoint", "A BHoM Geometry Point to compare with")]
        [Output("doPointsMatch", "True if the two points match on 2 out of 3 axes, false otherwise")]
        public static bool MatchPointOn2Of3(this Point point, Point comparePoint)
        {
            if(point == null || comparePoint == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query whether two points match on 2 out of 3 axes if either point is null.");
                return false;
            }

            bool match2 = false;
            if (point.X == comparePoint.X)
            {
                if (point.Y == comparePoint.Y)
                    match2 = true;
                else if (point.Z == comparePoint.Z)
                    match2 = true;
            }
            else if (point.Y == comparePoint.Y && point.Z == comparePoint.Z)
                match2 = true;

            return match2;
        }
    }
}






