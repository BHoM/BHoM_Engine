/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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

using BH.oM.Reflection.Attributes;

using BH.oM.Environment.Elements;
using BH.oM.Geometry;
using BH.Engine.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Compute
    {
        [Description("Map spaces based on geometry to an original set of spaces. This is done by taking the centre point of the space perimeter and checking which original space contains that centre point. E.G. mapping IES zoned spaces back to original Revit spaces.")]
        [Input("spacesToMap", "A collection of Environment spaces to map to original spaces")]
        [Input("originalSpaces", "A collection of original spaces to map to")]
        [Output("mappedSpaces", "A nested list of spaces mapped to the originals")]
        public static List<List<Space>> MapSpaces(List<Space> spacesToMap, List<Space> originalSpaces)
        {    
            List<List<Space>> data = new List<List<Space>>();

            foreach (Space space in originalSpaces)
            {
                data.Add(new List<Space>());
            }

            foreach (Space space in spacesToMap)   
            {
                Point centre = space.Perimeter.ICentroid();
                Space matching = originalSpaces.Where(x => x.Perimeter.IIsContaining(new List<Point> { centre })).First();

                data[originalSpaces.IndexOf(matching)].Add(space);
            }

            return data;
        }
    }
}