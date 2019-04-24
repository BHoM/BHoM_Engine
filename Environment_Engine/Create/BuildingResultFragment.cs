/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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

using BH.oM.Environment.Properties;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Create
    {
        [Description("Returns a Building Result Fragment object")]
        [Input("name", "The name of the fragment property, default empty string")]
        [Input("peakCooling", "The peak cooling result for the building, default 0.0")]
        [Input("peakHeating", "The peak heating result for the building, default 0.0")]
        [Output("buildingResultFragment", "An Environment Building Result Fragment object - this can be added to an Environment Building")]
        public static BuildingResultFragment BuildingResultFragment(string name = "", double peakCooling = 0.0, double peakHeating = 0.0)
        {
            return new BuildingResultFragment
            {
                Name = name,
                PeakCooling = peakCooling,
                PeakHeating = peakHeating,
            };
        }
    }
}
