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

using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/

        [Description("Thicken a line to create a closed polyline with the same direction but also a custom width.")]
        [Input("line", "A line to convert into a closed polyline via thickening.")]
        [Input("width", "A custom width for the new closed polyline.")]
        [Output("polyline", "A closed polyline with the same direction as the input line but also a custom width.")]
        public static Polyline Thicken(this Line line, double width)
        {
            var l1 = line.Offset(width / 2, Vector.ZAxis);
            var l2 = line.Offset(width / 2, -Vector.ZAxis);
            var cPnts = new List<Point> { l1.Start, l1.End, l2.End, l2.Start };

            return new Polyline { ControlPoints = cPnts }.Close();
        }

        /***************************************************/
    }
}



