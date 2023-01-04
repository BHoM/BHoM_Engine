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

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Base;
using BH.oM.Geometry;
using BH.oM.Data.Collections;

namespace BH.Engine.Data
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a numerical domain around the two values.")]
        [Input("min", "The lowest value in the domain, will be set as max if larger than max.")]
        [Input("max", "The highest value in the domain, will be set as min if larger than min.")]
        [Output("domain", "A numerical domain with its extreme values at min and max.")]
        public static Domain Domain(double min, double max)
        {
            return new Domain(Math.Min(min, max), Math.Max(min, max));
        }

        /***************************************************/

        [Description("Creates a numerical domain around the collection of values.")]
        [Input("values", "Values to create a domain around.")]
        [Output("domain", "A numerical domain enclosing all the values of the input.")]
        public static Domain Domain(IEnumerable<double> values)
        {
            if (!values.Any())
                return null;

            double min = values.First();
            double max = min;

            foreach (double value in values.Skip(1))
            {
                if (value < min)
                    min = value;
                else if (value > max)
                    max = value;
            }

            return new Domain(min, max);
        }

        /***************************************************/

    }
}



