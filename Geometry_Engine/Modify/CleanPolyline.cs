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

using BH.oM.Geometry;

using System.Collections.Generic;
using System.Linq;

using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a polyline that is cleaned by removing least significant vertices and short segments. This is designed for closed polylines only")]
        [Input("polyline", "The polyline you wish to clean by removing unnecessary points")]
        [Input("angleTolerance", "The tolerance of the angle that defines a straight line. Default is set to the value defined by BH.oM.Geometry.Tolerance.Angle")]
        [Input("minimumSegmentLength", "The length of the smallest allowed segment. Segments smaller than this will be removed. Default is set to the value defined by BH.oM.Geometry.Tolerance.Distance")]
        [Output("polyline", "The cleaned polyline")]
        public static Polyline CleanPolyline(this Polyline polyline, double angleTolerance = Tolerance.Angle, double minimumSegmentLength = Tolerance.Distance)
        {
            return polyline.RemoveLeastSignificantVertices(angleTolerance, angleTolerance, minimumSegmentLength).RemoveShortSegments(minimumSegmentLength, minimumSegmentLength);
        }
    }
}






