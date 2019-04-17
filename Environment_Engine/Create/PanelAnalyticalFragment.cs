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
        [Description("BH.Engine.Environment.Create.PanelAnalyticalFragment => Returns an Panel Analytical Fragment object")]
        [Input("name", "The name of the fragment property, default empty string")]
        [Input("altitude", "The altitude of the panel, default 0.0")]
        [Input("altitudeRange", "The altitude range of the panel, default 0.0")]
        [Input("inclination", "The inclination of the panel, default 0.0")]
        [Input("inclinationRange", "The inclination range of the panel, default 0.0")]
        [Input("orientation", "The orientation of the panel, default 0.0")]
        [Input("gValue", "The gValue of the panel, default 0.0")]
        [Input("ltValue", "The ltValue of the panel, default 0.0")]
        [Input("uValue", "The uValue of the panel, default 0.0")]
        [Input("apertureFlowIn", "The aperture flow in towards the panel, default 0.0")]
        [Input("apertureFlowOut", "The aperture flow out from the panel, default 0.0")]
        [Input("apertureOpening", "The aperture for the opening of the panel, default 0.0")]
        [Input("externalCondensation", "The external condensation for the panel, default 0.0")]
        [Input("externalConduction", "The external conduction for the panel, default 0.0")]
        [Input("externalConvection", "The external convection for the panel, default 0.0")]
        [Input("externalLongWave", "The external long wave for the panel, default 0.0")]
        [Input("externalSolar", "The external solar result of the panel, default 0.0")]
        [Input("externalTemperature", "The external temperature of the panel, default 0.0")]
        [Input("internalCondensation", "The internal condensation for the panel, default 0.0")]
        [Input("internalConduction", "The internal conduction for the panel, default 0.0")]
        [Input("internalConvection", "The internal convection for the panel, default 0.0")]
        [Input("internalLongWave", "The internal long wave for the panel, default 0.0")]
        [Input("internalSolar", "The internal solar result of the panel, default 0.0")]
        [Input("internalTemperature", "The internal temperature of the panel, default 0.0")]
        [Input("interstitialCondensation", "The interstitial condensation of the panel, default 0.0")]
        [Output("A Panel Analytical Fragment object - this can be added to an Environment Panel")]
        public static PanelAnalyticalFragment PanelAnalyticalFragment(string name = "", double altitude = 0.0, double altitudeRange = 0.0, double inclination = 0.0, double inclinationRange = 0.0, double orientation = 0.0, double gValue = 0.0, double ltValue = 0.0, double uValue = 0.0, double apertureFlowIn = 0.0, double apertureFlowOut = 0.0, double apertureOpening = 0.0, double externalCondensation = 0.0, double externalConduction = 0.0, double externalConvection = 0.0, double externalLongWave = 0.0, double externalSolar = 0.0, double externalTemperature = 0.0, double internalCondensation = 0.0, double internalConduction = 0.0, double internalConvection = 0.0, double internalLongWave = 0.0, double internalSolar = 0.0, double internalTemperature = 0.0, double interstitialCondensation = 0.0)
        {
            return new PanelAnalyticalFragment
            {
                Name = name,
                Altitude = altitude,
                AltitudeRange = altitudeRange,
                Inclination = inclination,
                InclinationRange = inclinationRange,
                Orientation = orientation,
                GValue = gValue,
                LTValue = ltValue,
                UValue = uValue,
                ApertureFlowIn = apertureFlowIn,
                ApertureFlowOut = apertureFlowOut,
                ApertureOpening = apertureOpening,
                ExternalCondensation = externalCondensation,
                ExternalConduction = externalConduction,
                ExternalConvection = externalConvection,
                ExternalLongWave = externalLongWave,
                ExternalSolar = externalSolar,
                ExternalTemperature = externalTemperature,
                InternalCondensation = internalCondensation,
                InternalConduction = internalConduction,
                InternalConvection = internalConvection,
                InternalLongWave = internalLongWave,
                InternalSolar = internalSolar,
                InternalTemperature = internalTemperature,
                InterstitialCondensation = interstitialCondensation,
            };
        }
    }
}
