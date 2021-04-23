/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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
using System.ComponentModel;

using BH.Engine.Geometry;
using BH.oM.Geometry;
using BH.Engine.Base;
using BH.oM.Reflection.Attributes;
using BH.oM.Environment.Elements;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a list of Environment Spaces with the provided spacetype assigned by a string and a point in the space")]
        [Input("spaces", "A collection of Environment Spaces to set the type for")]
        [Input("searchPoints", "A collection of points to search")]
        [Input("type", "The type of space to assign")]
        [Input("ignoreCase", "Whether or not the parse will be casesensitive")]
        [Output("modifiedSpaces", "A collection of modified Environment Spaces with asssigned space types")]
        public static List<Space> AssignSpaceType(this List<Space> spaces, List<Point> searchPoints, string type, bool ignoreCase = true)
        {
            List<Space> updateSpaces = new List<Space>();
            foreach (Space s in spaces)
                updateSpaces.Add(s.DeepClone());

            SpaceType spaceType = SpaceType.Undefined;
            object value = Enum.Parse(typeof(SpaceType), type, ignoreCase);
            if (value != null)
                spaceType = (SpaceType)value;

            List<Space> returnSpaces = new List<Space>();
            for (int x = 0; x < searchPoints.Count; x++)
            {
                Space update = updateSpaces.Where(a => a.Perimeter.IIsContaining(new List<Point> { searchPoints[x] })).FirstOrDefault();
                if (update == null)
                    continue;

                update.SpaceType = spaceType;
                returnSpaces.Add(update);
            }

            return returnSpaces;
        }
    }
}