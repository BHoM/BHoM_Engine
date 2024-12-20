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

using System.ComponentModel;

using BH.oM.Geometry;
using BH.oM.MEP.System;
using BH.oM.MEP.Fixtures;
using BH.oM.Base.Attributes;

namespace BH.Engine.MEP
{
    public static partial class Query
    {
        /***************************************************/
        /****              Public Methods               ****/
        /***************************************************/
        [Description("Returns the centreline of any IFlow object as the line between the StartPoint and EndPoint. No offsets or similar is accounted for.")]
        [Input("flowObj", "The IFlow object to get the centreline from.")]
        [Output("centreline", "The centreline of the IFlow object.")]
        public static Line Centreline(this IFlow flowObj)
        {
            if(flowObj == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the centreline of a null flow object.");
                return null;
            }

            return new Line { Start = flowObj.StartPoint, End = flowObj.EndPoint };
        }

        /***************************************************/
    }
}






