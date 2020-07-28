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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Geometry.ShapeProfiles;

namespace BH.Engine.MEP
{
    public static partial class Query
    {
        [Description("Returns the Circular Equivalent Diameter for ducts that are non-circular, equivalent in length, fluid resistance and airflow")]
        public static double ICircularEquivalentDiameter(this IProfile profile)
        {
            return CircularEquivalentDiameter(profile as dynamic);
        }

        public static double CircularEquivalentDiameter(this BoxProfile box)
        {
            double a = 1000 * (box.Height - 2 * box.Thickness);
            double b = 1000 * (box.Width - 2 * box.Thickness);
            return (1.30 * Math.Pow(a * b, 0.625) / Math.Pow(a + b, 0.250)) / 1000;
        }
        private static double CircularEquivalentDiameter(this IProfile profile)
        {
            return -1;
        }
    }
}
