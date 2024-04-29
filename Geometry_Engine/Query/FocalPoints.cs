/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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

using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Geometry
{ 
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the two focal points of the Ellipse. For all points on the ellipse, the sum of the two distances to the focal points is constant.")]
        [Input("ellipse", "The ellipse to extract the two focal points from.")]
        [MultiOutput(0, "f1", "First focal point.")]
        [MultiOutput(1, "f2", "Second focal point.")]
        public static Output<Point,Point> FocalPoints(this Ellipse ellipse)
        {
            if(ellipse.IsNull())
                return null;

            //Get the major axis (axis with largest radius)
            Vector va;
            double a, b;
            if (ellipse.Radius1 > ellipse.Radius2)
            {
                a = ellipse.Radius1;
                b = ellipse.Radius2;
                va = ellipse.CoordinateSystem.X;
            }
            else
            {
                a = ellipse.Radius2;
                b = ellipse.Radius1;
                va = ellipse.CoordinateSystem.Y;
            }

            //Distance from centre
            double c = Math.Sqrt(a * a - b * b);    
            va = va * c;

            return new Output<Point, Point> { Item1 = ellipse.CoordinateSystem.Origin + va, Item2 = ellipse.CoordinateSystem.Origin - va };
        }

        /***************************************************/
    }
}

