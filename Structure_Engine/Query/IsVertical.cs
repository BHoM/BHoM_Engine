/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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
using BH.oM.Structure.Elements;
using BH.Engine.Geometry;
using System;

using BH.oM.Reflection.Attributes;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool IsVertical(this Bar bar)
        {
            return Engine.Geometry.Query.IsVertical(bar.Centreline());
        }

        /***************************************************/

        [Deprecated("3.1", "Moved to Geometry_Engine")]
        public static bool IsVertical(this Line line)
        {
            return Engine.Geometry.Query.IsVertical(line);
        }

        /***************************************************/

        [Deprecated("2.3", "Methods replaced with methods targeting BH.oM.Physical.Elements.IFramingElement")]
        public static bool IsVertical(this FramingElement element)
        {
            return Engine.Geometry.Query.IsVertical(new Line() { Start = element.LocationCurve.IStartPoint(), End = element.LocationCurve.IEndPoint() } ); //TODO: is this correct? what is the framing element is curved?
        }

        /***************************************************/

    }
}