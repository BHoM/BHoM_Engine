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

using BH.oM.Environment.Fragments;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Create
    {
        [Deprecated("3.0", "Deprecated to remove name input", null, "LightTransmittanceFragment(redReflectance, greenReflectance, blueReflectance)")]
        public static LightTransmittanceFragment LightTransmittanceFragment(string name = "", double redTransmittance = 0.0, double greenTransmittance = 0.0, double blueTransmittance = 0.0)
        {
            return Create.LightTransmittanceFragment(redTransmittance, greenTransmittance, blueTransmittance);
        }

        [Description("Returns an Environment Light Transmittance Fragment object")]
        [Input("redTransmittance", "The red transmittance of the light transmittance, default 0.0")]
        [Input("greenTransmittance", "The green transmittance of the light transmittance, default 0.0")]
        [Input("blueTransmittance", "The blue transmittance of the light transmittance, default 0.0")]
        [Output("lightTransmittanceFragment", "A Light Transmittance Fragment object - this can be added to an Environmental Material fragment object")]
        [Deprecated("3.0", "Deprecated in favour of default create components produced by BHoM")]
        public static LightTransmittanceFragment LightTransmittanceFragment(double redTransmittance = 0.0, double greenTransmittance = 0.0, double blueTransmittance = 0.0)
        {
            return new LightTransmittanceFragment
            {
                RedTransmittance = redTransmittance,
                GreenTransmittance = greenTransmittance,
                BlueTransmittance = blueTransmittance,
            };
        }
    }
}

