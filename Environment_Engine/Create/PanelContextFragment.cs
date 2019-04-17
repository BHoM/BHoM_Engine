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
        [Description("BH.Engine.Environment.Create.PanelContextFragment => Returns an Panel Context Fragment object")]
        [Input("name", "The name of the fragment property, default empty string")]
        [Input("isAir", "Defines whether the panel is an air panel, default false")]
        [Input("isGround", "Defines whether the panel is a ground panel, default false")]
        [Input("colour", "Defines the colour of the panel, default empty string")]
        [Input("reversed", "Defines whether the panel is reversed, default false")]
        [Output("A Panel Context Fragment object - this can be added to an Environment Panel")]
        public static PanelContextFragment PanelContextFragment(string name = "", bool isAir = false, bool isGround = false, string colour = "", bool reversed = false)
        {
            return new PanelContextFragment
            {
                Name = name,
                IsAir = isAir,
                IsGround = isGround,
                Colour = colour,
                Reversed = reversed,
            };
        }
    }
}
