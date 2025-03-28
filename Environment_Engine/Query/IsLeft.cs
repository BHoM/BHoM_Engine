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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        [Description("Determine whether a point falls on the left side of a line or not. The left side is defined as the left hand side of the line when standing on the start point and looking at the end point")]
        [Input("line", "The line to determine directionality")]
        [Input("check", "The point to check against")]
        [Output("isLeft", "True if the point is on the left hand side of the line. False if it is on the line or on the right hand side")]
        public static bool IsLeft(this Line line, Point check)
        {
            if(line == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query whether a point is on the left side of a null line.");
                return false;
            }

            if(check == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query whether a point is on the left side of a line if the point is null.");
                return false;
            }

            return ((line.End.X - line.Start.X) * (check.Y - line.Start.Y) - (line.End.Y - line.Start.Y) * (check.X - line.Start.X)) > 0;
        }
    }
}





