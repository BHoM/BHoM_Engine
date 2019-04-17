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
        [Description("BH.Engine.Environment.Create.BuildingAnalyticalFragment => Returns a Building Analytical Fragment object")]
        [Input("name", "The name of the fragment property, default empty string")]
        [Input("northAngle", "The angle to north for the building fragment, default 0.0")]
        [Input("gmtOffset", "The timezone of the building as an offset to GMT in decimal hours, default 0.0")]
        [Input("year", "The year of the building to be analysed, default 0")]
        [Output("An Environment Analytical Building Fragment object - this can be added to an Environment Building")]
        public static BuildingAnalyticalFragment BuildingAnalyticalFragment(string name = "", double northAngle = 0.0, double gmtOffset = 0.0, int year = 0)
        {
            return new BuildingAnalyticalFragment
            {
                Name = name,
                NorthAngle = northAngle,
                GMTOffset = gmtOffset,
                Year = year,
            };
        }
    }
}
