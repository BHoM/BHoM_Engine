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

using BH.oM.Environment.Gains;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns an Environment Emitter object")]
        [Input("name", "The name of the emitter, default empty string")]
        [Input("radiantProportion", "The radiant proportion of the emitter, default 0.0")]
        [Input("viewCoefficient", "The view coefficient of the emitter, default 0.0")]
        [Input("maximumOutsideTemperature", "The maximum temperature outside the space the emitter should be working with, default 0.0")]
        [Input("switchOffOutsideTemperature", "The amount of temperature to be used outside the emitter when switched off, default 0.0")]
        [Input("type", "The type of emitter from the Emitter Type enum, default undefined")]
        [Output("emitter", "An Environment Emitter object")]
        public static Emitter Emitter(string name = "", double radiantProportion = 0.0, double viewCoefficient = 0.0, double maximumOutsideTemperature = 0.0, double switchOffOutsideTemperature = 0.0, EmitterType type = EmitterType.Undefined)
        {
            return new Emitter
            {
                Name = name,
                RadiantProportion = radiantProportion,
                ViewCoefficient = viewCoefficient,
                MaximumOutsideTemperature = maximumOutsideTemperature,
                SwitchOffOutsideTemperature = switchOffOutsideTemperature,
                Type = type,
            };
        }
    }
}
