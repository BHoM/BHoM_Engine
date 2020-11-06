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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

using BH.oM.MEP.Equipment.Parts;

namespace BH.Engine.MEP
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns an MEP Filter part")]
        [Input("type", "Default 0")]
        [Input("mervRating", "Default 0")]
        [Input("initialPressureDrop", "Default 0")]
        [Input("replacementPressureDrop", "Default 0")]
        [Input("area", "Default 0")]
        [Output("filter", "An MEP Filter part")]
        public static Filter Filter(string type = "", int mervRating = 0, double initialPressureDrop = 0.0, double replacementPressureDrop = 0.0, double area = 0)
        {
            return new Filter
            {
                Type = type,
                MERVRating = mervRating,
                InitialPressureDrop = initialPressureDrop,
                ReplacementPressureDrop = replacementPressureDrop,
                Area = area,
            };
        }
    }
}

