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

using BH.oM.Environment.Fragments;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Create
    {
        [Deprecated("3.0", "Deprecated to remove name input", null, "SpaceAnalyticalFragment(internalDomesticHotWater, daylightFactor, facadeLength, fixedConvectionCoefficient, sizeCooling, sizeHeating, radiantProportion)")]
        public static SpaceAnalyticalFragment SpaceAnalyticalFragment(string name = "", double internalDomesticHotWater = 0.0, double daylightFactor = 0.0, double facadeLength = 0.0, double fixedConvectionCoefficient = 0.0, SizingMethod sizeCooling = SizingMethod.Undefined, SizingMethod sizeHeating = SizingMethod.Undefined, double radiantProportion = 0.0)
        {
            return Create.SpaceAnalyticalFragment(internalDomesticHotWater, daylightFactor, facadeLength, fixedConvectionCoefficient, sizeCooling, sizeHeating, radiantProportion);
        }

        [Description("Returns an Space Analytical Fragment object")]
        [Input("internalDomesticHotWater", "The amount of internal domestic hot water supply for the space, default 0.0")]
        [Input("daylightFactor", "The daylight factor for the space, default 0.0")]
        [Input("facadeLength", "The length of the facade on the space, default 0.0")]
        [Input("fixedConvectionCoefficient", "The fixed convection coefficient of the space, default 0.0")]
        [Input("sizeCooling", "The cooling size method of the space from the Sizing Method enum, default undefined")]
        [Input("sizeHeating", "The heating size method of the space from the Sizing Method enum, default undefined")]
        [Input("radiantProportion", "The radiant proportion of the space, default 0.0")]
        [Output("spaceAnalyticalFragment", "A Space Analytical Fragment object - this can be added to an Environment Space")]
        [Deprecated("3.0", "Deprecated in favour of default create components produced by BHoM")]
        public static SpaceAnalyticalFragment SpaceAnalyticalFragment(double internalDomesticHotWater = 0.0, double daylightFactor = 0.0, double facadeLength = 0.0, double fixedConvectionCoefficient = 0.0, SizingMethod sizeCooling = SizingMethod.Undefined, SizingMethod sizeHeating = SizingMethod.Undefined, double radiantProportion = 0.0)
        {
            return new SpaceAnalyticalFragment
            {
                InternalDomesticHotWater = internalDomesticHotWater,
                DaylightFactor = daylightFactor,
                FacadeLength = facadeLength,
                FixedConvectionCoefficient = fixedConvectionCoefficient,
                SizeCoolingMethod = sizeCooling,
                SizeHeatingMethod = sizeHeating,
                RadiantProportion = radiantProportion,
            };
        }
    }
}
