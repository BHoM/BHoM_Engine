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
        [Description("Create a Equipment Gain object which can be attributed to a space")]
        [Input("sensible", "The sensible gain load for the equipment gain, default 0.0")]
        [Input("latent", "The latent gain load for the equipment gain, default 0.0")]
        [Input("profile", "The profile for this gain being active, default null")]
        [Input("radiantFraction", "The fraction of radiance from this equipment gain, default 0.0")]
        [Input("viewCoefficient", "The view coefficient of this equipment gain, default 0.0")]
        [Input("name", "The name of this equipment gain, default empty string")]
        [Output("equipmentGain", "The equipment gain object to associate to a space")]
        public static Equipment Equipment(double sensible = 0.0, double latent = 0.0, Profile profile = null, double radiantFraction = 0.0, double viewCoefficient = 0.0, string name = "")
        {
            return new Equipment
            {
                Sensible = sensible,
                Latent = latent,
                Profile = profile,
                RadiantFraction = radiantFraction,
                ViewCoefficient = viewCoefficient,
                Name = name,
            };
        }

        [Description("Create an Infiltration Gain object which can be attributed to a space")]
        [Input("sensible", "The sensible gain load for the infiltration gain, default 0.0")]
        [Input("latent", "The latent gain load for the infiltration gain, default 0.0")]
        [Input("profile", "The profile for this gain being active, default null")]
        [Input("name", "The name of this infiltration gain, default empty string")]
        [Output("infiltrationGain", "The infiltration gain object to associate to a space")]
        public static Infiltration Infiltration(double sensible = 0.0, double latent = 0.0, Profile profile = null, string name = "")
        {
            return new Infiltration
            {
                Sensible = sensible,
                Latent = latent,
                Profile = profile,
                Name = name,
            };
        }

        [Description("Create a Lighting Gain object which can be attributed to a space")]
        [Input("sensible", "The sensible gain load for the lighting gain, default 0.0")]
        [Input("profile", "The profile for this gain being active, default null")]
        [Input("radiantFraction", "The fraction of radiance from this lighting gain, default 0.0")]
        [Input("viewCoefficient", "The view coefficient of this lighting gain, default 0.0")]
        [Input("luminousEfficacy", "The luminous efficacy of this lighting gain, default 0.0")]
        [Input("name", "The name of this lighting gain, default empty string")]
        [Output("lightingGain", "The lighting gain object to associate to a space")]
        public static Lighting Lighting(double sensible = 0.0, Profile profile = null, double radiantFraction = 0.0, double viewCoefficient = 0.0, double luminousEfficiacy = 0.0, string name = "")
        {
            return new Lighting
            {
                Sensible = sensible,
                Profile = profile,
                RadiantFraction = radiantFraction,
                ViewCoefficient = viewCoefficient,
                LuminousEfficacy = luminousEfficiacy,
                Name = name,
            };
        }

        [Description("Create a People Gain object which can be attributed to a space")]
        [Input("sensible", "The sensible gain load for the people gain, default 0.0")]
        [Input("latent", "The latent gain load for the people gain, default 0.0")]
        [Input("profile", "The profile for this gain being active, default null")]
        [Input("radiantFraction", "The fraction of radiance from this people gain, default 0.0")]
        [Input("viewCoefficient", "The view coefficient of this people gain, default 0.0")]
        [Input("name", "The name of this people gain, default empty string")]
        [Output("peopleGain", "The people gain object to associate to a space")]
        public static People People(double sensible = 0.0, double latent = 0.0, Profile profile = null, double radiantFraction = 0.0, double viewCoefficient = 0.0, string name = "")
        {
            return new People
            {
                Sensible = sensible,
                Latent = latent,
                Profile = profile,
                RadiantFraction = radiantFraction,
                ViewCoefficient = viewCoefficient,
                Name = name,
            };
        }

        [Description("Create a Plug Gain object which can be attributed to a space")]
        [Input("sensible", "The sensible gain load for the plug gain, default 0.0")]
        [Input("profile", "The profile for this gain being active, default null")]
        [Input("radiantFraction", "The fraction of radiance from this people gain, default 0.0")]
        [Input("viewCoefficient", "The view coefficient of this plug gain, default 0.0")]
        [Input("name", "The name of this plug gain, default empty string")]
        [Output("plugGain", "The plug gain object to associate to a space")]
        public static Plug Plug(double sensible = 0.0, Profile profile = null, double radiantFraction = 0.0, double viewCoefficient = 0.0, string name = "")
        {
            return new Plug
            {
                Sensible = sensible,
                Profile = profile,
                RadiantFraction = radiantFraction,
                ViewCoefficient = viewCoefficient,
                Name = name,
            };
        }

        [Description("Create a Pollutant Gain object which can be attributed to a space")]
        [Input("sensible", "The sensible gain load for the pollutant gain, default 0.0")]
        [Input("latent", "The latent gain load for the pollutant gain, default 0.0")]
        [Input("profile", "The profile for this gain being active, default null")]
        [Input("name", "The name of this pollutant gain, default empty string")]
        [Output("pollutantGain", "The pollutant gain object to associate to a space")]
        public static Pollutant Pollutant(double sensible = 0.0, double latent = 0.0, Profile profile = null, string name = "")
        {
            return new Pollutant
            {
                Sensible = sensible,
                Latent = latent,
                Profile = profile,
                Name = name,
            };
        }
    }
}
