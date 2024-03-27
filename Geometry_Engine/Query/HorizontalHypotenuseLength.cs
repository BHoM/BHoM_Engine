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

using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using System;
using System.ComponentModel;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {

        /***************************************************/
        /****       Public Methods - BoundingBox        ****/
        /***************************************************/

        [Description("Returns the horizontal hypotenuse length of a BHoM BoundingBox.")]
        [Input("boundingBox", "BHoM BoundingBox to query its hypotenuse length.")]
        [Output("width", "The horizontal hypotenuse length of the BoundingBox based on the difference in XY values for its bounding box.",typeof(Length))]
        public static double HorizontalHypotenuseLength(this BoundingBox boundingBox)
        {
            if (boundingBox == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the hypotenuse length of a null bounding box.");
                return 0;
            }
            
            double diffX = Math.Abs(boundingBox.Max.X - boundingBox.Min.X);
            double diffY = Math.Abs(boundingBox.Max.Y - boundingBox.Min.Y);

            return Math.Sqrt((diffX * diffX) + (diffY * diffY));
        }       

        /***************************************************/
    }
}



