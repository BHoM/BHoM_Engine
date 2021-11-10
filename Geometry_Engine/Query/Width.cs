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
using BH.oM.Reflection.Attributes;
using System;
using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {

        /***************************************************/
        /****         Public Methods - ICurve           ****/
        /***************************************************/

        [Description("Returns the width of a BHoM Geometry ICurve based on the bounding box of the curve")]
        [Input("curve", "BHoM Geometry ICurve")]
        [Output("width", "The width of the curve based on the difference in XY values for its bounding box")]
        public static double Width(this ICurve curve)
        {
            if (curve == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot query the width of a null curve.");
                return -1;
            }

            BoundingBox bBox = curve.IBounds();

            double diffX = Math.Abs(bBox.Max.X - bBox.Min.X);
            double diffY = Math.Abs(bBox.Max.Y - bBox.Min.Y);

            return Math.Sqrt((diffX * diffX) + (diffY * diffY));
        }       

        /***************************************************/
    }
}
