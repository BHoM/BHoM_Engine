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

using BH.oM.Geometry;
using BH.oM.MEP.System;

namespace BH.Engine.MEP
{
    public static partial class Query
    {
        /***************************************************/
        /****             Public Methods                ****/
        /***************************************************/
        public static IGeometry Geometry(this Duct duct)
        {
            return new Line { Start = duct.StartPoint.Position, End = duct.EndPoint.Position};
        }

        /***************************************************/
        public static IGeometry Geometry(this BH.oM.MEP.System.Pipe pipe)
        {
            return new Line { Start = pipe.StartPoint.Position, End = pipe.EndPoint.Position };
        }

        /***************************************************/
        public static IGeometry Geometry(this WireSegment wire)
        {
            return new Line { Start = wire.StartPoint.Position, End = wire.EndPoint.Position };
        }

        /***************************************************/
        public static IGeometry Geometry(this CableTray cableTray)
        {
            return new Line { Start = cableTray.StartPoint.Position, End = cableTray.EndPoint.Position };
        }

        /***************************************************/
    }
}
