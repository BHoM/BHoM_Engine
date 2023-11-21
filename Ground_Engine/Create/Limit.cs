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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using BH.oM.Geometry.CoordinateSystem;
using BH.oM.Ground;
using BH.Engine.Base;
using BH.Engine.Geometry;

namespace BH.Engine.Ground
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a Limit object to compare against results.")]
        [Input("value", "The value of the limit.")]
        [Input("name", "The name of the limit (this can refer to the parameter to be compared against).")]
        [Input("author", "The author of the limit.")]
        [Input("reference", "Any references for the limit (e.g. a design code).")]
        [Input("maxima", "Is the limit a maxima? True if a maxima, false if a minima.")]
        [Output("limit", "The limit object to compare against results.")]
        public static Limit Limit(double value, string name, string author = "", string reference = "", bool maxima = true)
        {
            if (double.IsNaN(value))
            {
                Base.Compute.RecordError("The value for the limit must be a number.");
                return null;
            }

            if(name == "")
            {
                Base.Compute.RecordError("The name for the limit cannot be empty");
                return null;
            }

            return new Limit()
            {
                Value = value,
                Name = name,
                Author = author,
                Reference = reference,
                Maxima = maxima,
            };
        }

        /***************************************************/
    }
}




