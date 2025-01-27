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

        [Description("Creates a DomainBox from the two value arrays.")]
        [Input("min", "The minimum value for each dimension.")]
        [Input("max", "The maximum value for each dimension.")]
        [Output("domainBox", "A DomainBox enclosing the values.")]
        public static DomainBox DomainBox(double[] min, double[] max)
        {
            return DomainBox(min.Zip(max, (x,y) => new double[] { x, y }));
        }

        /***************************************************/

        [Description("Creates a DomainBox enclosing each array in the collection.")]
        [Input("values", "Each array in the collection is for a different dimension and will be enclosed by the DomainBox.")]
        [Output("domainBox", "A DomainBox enclosing the values.")]
        public static DomainBox DomainBox(IEnumerable<double[]> values)
        {
            if (!values.Any())
                return null;

            DomainBox result = new DomainBox()
            {
                Domains = values.Select(x => Domain(x)).ToArray()
            };

            if (result.Domains.Any(x => x == null))
                return null;

            return result;
        }

        /***************************************************/

    }
}





