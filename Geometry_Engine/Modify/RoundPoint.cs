/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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

using BH.oM.Geometry;
using System.ComponentModel;
using BH.oM.Base.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        [Deprecated("3.2", "Renamed to RoundCoordinates and expanded for other Geometry", null, "BH.Engine.Geometry.Modify.RoundCoordinates")]
        [Description("Modifies a BHoM Geometry Point to be rounded to the number of provided decimal places")]
        [Input("point", "The BHoM Geometry Point to modify")]
        [Input("decimalPlaces", "The number of decimal places to round to, default 6")]
        [Output("point", "The modified BHoM Geometry Point")]
        public static Point RoundPoint(this Point point, int decimalPlaces = 6)
        {
            return RoundCoordinates(point, decimalPlaces);
        }
    }
}


