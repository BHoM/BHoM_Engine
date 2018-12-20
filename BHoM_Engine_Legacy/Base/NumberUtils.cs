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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.Base
{
    public static class NumberUtils
    {

        public static bool InRange(double value, double upper, double lower, double tolerance)
        {
            return value < upper + tolerance && value > lower - tolerance;
        }

        public static bool InBetween(double value, double b1, double b2, double tolerance)
        {
            if (b1 > b2)
            {
                return InRange(value, b1, b2, tolerance);
            }
            else
            {
                return InRange(value, b2, b1, tolerance);
            }
        }

        public static bool NearEqual(double value1, double value2, double eps)
        {
            return value1 < value2 + eps && value1 > value2 - eps;
        }
    }
}
