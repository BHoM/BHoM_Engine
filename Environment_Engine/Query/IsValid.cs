/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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
using System.Text;
using BH.oM.Environment.Elements;
using BH.oM.Base;
using BH.Engine.Geometry;
using BH.Engine.Spatial;
using BH.Engine.Base;
using BH.oM.Spatial.Layouts;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        [Description("Checks if the space is valid by checking if the perimeter curve is closed, if the perimeter length is not equal to 0 and if the space perimeter is not self intersecting.")]
        [Input("spaces", "The Spaces to check if they are valid to the given conditions.")]
        [MultiOutput(0, "validSpaces", "Returns list of valid spaces.")]
        [MultiOutput(1, "selfIntersectingSpaces", "Returns list of invalid spaces due to the perimeter curve self intersecting.")]
        [MultiOutput(2, "zeroPerimeterSpaces", "Returns list of invalid spaces due to perimeter length being zero.")]
        [MultiOutput(3, "notClosedSpaces", "Returns list of invalid spaces due to not closed perimeter.")]
        public static Output<List<Space>, List<Space>, List<Space>, List<Space>> IsValid(this List<Space> spaces, double intersectionTolerance = BH.oM.Geometry.Tolerance.Distance, double lengthTolerance = BH.oM.Geometry.Tolerance.Distance, double closedSpacesTolerance = BH.oM.Geometry.Tolerance.Distance)
        {
            if(spaces == null)
            {
                BH.Engine.Base.Compute.RecordError("Spaces input cannot be null, please provide valid spaces.");
                return null;
            }
                
            List<Space> validSpaces = new List<Space>();
            List<Space> selfIntersectingSpaces = new List<Space>();
            List<Space> zeroPerimeterSpaces = new List<Space>();
            List<Space> notClosedSpaces = new List<Space>();
            foreach (Space space in spaces)
            {
                if(space == null)
                    continue;
                    
                bool isvalid = true;
                if (space.IsSelfIntersecting(intersectionTolerance))
                {
                    selfIntersectingSpaces.Add(space);
                    isvalid = false;
                }

                double length = space.Perimeter.Length();
                if ((length >= (0 - lengthTolerance)) && (length <= (0 + lengthTolerance)))
                {
                    zeroPerimeterSpaces.Add(space);
                    isvalid = false;
                }

                if (!space.Perimeter.IIsClosed(closedSpacesTolerance))
                {
                    notClosedSpaces.Add(space);
                    isvalid = false;
                }

                if (isvalid)
                {
                    validSpaces.Add(space);
                }
            }

            return new Output<List<Space>, List<Space>, List<Space>, List<Space>>{Item1 = validSpaces, Item2 = selfIntersectingSpaces, Item3 = notClosedSpaces, Item4 = zeroPerimeterSpaces};
        }
    }
}