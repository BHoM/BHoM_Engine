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

        /*[Description("Returns an Environment Gain object")]
        [Input("name", "The name of the gain, default empty string")]
        [Input("type", "The type of gain from the Gain Type enum, default undefined")]
        [Input("gainProperties", "The properties of the gain, for example, occupant gain properties, default null")]
        [Output("gain", "An Environment Gain object")]
        public static Gain Gain(string name = "", GainType type = GainType.Undefined, IGainProperties gainProperties = null)
        {
            switch(type)
            {
                case GainType.Equipment:
                    if (gainProperties.GetType() != typeof(LatentEquipmentGain) && gainProperties.GetType() != typeof(SensibleEquipmentGain))
                        BH.Engine.Reflection.Compute.RecordWarning("The gain properties you have supplied do not match the specified gain type");
                    break;
                case GainType.Infiltration:
                    if(gainProperties.GetType() != typeof(InfiltrationGain))
                        BH.Engine.Reflection.Compute.RecordWarning("The gain properties you have supplied do not match the specified gain type");
                    break;
                case GainType.Lighting:
                    if(gainProperties.GetType() != typeof(LightingGain))
                        BH.Engine.Reflection.Compute.RecordWarning("The gain properties you have supplied do not match the specified gain type");
                    break;
                case GainType.People:
                    if(gainProperties.GetType() != typeof(PeopleGain))
                        BH.Engine.Reflection.Compute.RecordWarning("The gain properties you have supplied do not match the specified gain type");
                    break;
                case GainType.Pollutant:
                    if(gainProperties.GetType() != typeof(PollutantGain))
                        BH.Engine.Reflection.Compute.RecordWarning("The gain properties you have supplied do not match the specified gain type");
                    break;
            }

            return new Gain
            {
                Name = name,
                Type = type,
                Properties = gainProperties,
            };
        }

        [Description("Returns an Environment Gain object with Infiltration Gain properties")]
        [Input("name", "The name of the infiltration gain, default empty string")]
        [Input("profile", "The profile to be used for this gain, default null")]
        [Input("unit", "The unit type of the gain from the Gain Unit enum, default undefined")]
        [Input("value", "The value the gain should provide to the space, default 0.0")]
        [Output("infiltrationGain", "An Environment Gain object")]
        public static Gain InfiltrationGain(string name = "", Profile profile = null, GainUnit unit = GainUnit.Undefined, double value = 0.0)
        {
            InfiltrationGain properties = new InfiltrationGain
            {
                Name = name,
                Profile = profile,
                Unit = unit,
                Value = value,
            };

            return Gain(name, GainType.Infiltration, properties);
        }

        [Description("Returns an Environment Gain object with Latent Equipment Gain properties")]
        [Input("name", "The name of the latent equipment gain, default empty string")]
        [Input("profile", "The profile to be used for this gain, default null")]
        [Input("unit", "The unit type of the gain from the Gain Unit enum, default undefined")]
        [Input("value", "The value the gain should provide to the space, default 0.0")]
        [Output("latentEquipmentGain", "An Environment Gain object")]
        public static Gain LatentEquipmentGain(string name = "", Profile profile = null, GainUnit unit = GainUnit.Undefined, double value = 0.0)
        {
            LatentEquipmentGain properties = new LatentEquipmentGain
            {
                Name = name,
                Profile = profile,
                Unit = unit,
                Value = value,
            };

            return Gain(name, GainType.Equipment, properties);
        }

        [Description("Returns an Environment Gain object with Lighting Gain properties")]
        [Input("name", "The name of the lighting gain, default empty string")]
        [Input("profile", "The profile to be used for this gain, default null")]
        [Input("unit", "The unit type of the gain from the Gain Unit enum, default undefined")]
        [Input("value", "The value the gain should provide to the space, default 0.0")]
        [Input("radiantFraction", "The fraction of radiance from this lighting gain, default 0.0")]
        [Input("viewCoefficient", "The view coefficient of this lighting gain, default 0.0")]
        [Input("luminousEfficacy", "The luminous efficacy of this lighting gain, default 0.0")]
        [Output("lightingGain", "An Environment Gain object")]
        public static Gain LightingGain(string name = "", Profile profile = null, GainUnit unit = GainUnit.Undefined, double value = 0.0, double radiantFraction = 0.0, double viewCoefficient = 0.0, double luminousEfficacy = 0.0)
        {
            LightingGain properties = new LightingGain
            {
                Name = name,
                Profile = profile,
                Unit = unit,
                Value = value,
                RadiantFraction = radiantFraction,
                ViewCoefficient = viewCoefficient,
                LuminousEfficacy = luminousEfficacy,
            };

            return Gain(name, GainType.Lighting, properties);
        }

        [Description("Returns an Environment Gain object with People Gain properties")]
        [Input("name", "The name of the people gain, default empty string")]
        [Input("profile", "The profile to be used for this gain, default null")]
        [Input("unit", "The unit type of the gain from the Gain Unit enum, default undefined")]
        [Input("value", "The value the gain should provide to the space, default 0.0")]
        [Input("radiantFraction", "The fraction of radiance from this people gain, default 0.0")]
        [Input("viewCoefficient", "The view coefficient of this people gain, default 0.0")]
        [Input("sensibleGain", "The sensible gain amount of this people gain, default 0.0")]
        [Input("latentGain", "The latent gain amount of this people gain, default 0.0")]
        [Output("peopleGain", "An Environment Gain object")]
        public static Gain PeopleGain(string name = "", Profile profile = null, GainUnit unit = GainUnit.Undefined, double value = 0.0, double radiantFraction = 0.0, double viewCoefficient = 0.0, double sensibleGain = 0.0, double latentGain = 0.0)
        {
            PeopleGain properties = new PeopleGain
            {
                Name = name,
                Profile = profile,
                Unit = unit,
                Value = value,
                RadiantFraction = radiantFraction,
                ViewCoefficient = viewCoefficient,
                SensibleGain = sensibleGain,
                LatentGain = latentGain,
            };

            return Gain(name, GainType.People, properties);
        }

        [Description("Returns an Environment Gain object with Pollutant Gain properties")]
        [Input("name", "The name of the pollutant gain, default empty string")]
        [Input("profile", "The profile to be used for this gain, default null")]
        [Input("unit", "The unit type of the gain from the Gain Unit enum, default undefined")]
        [Input("value", "The value the gain should provide to the space, default 0.0")]
        [Output("pollutantGain", "An Environment Gain object")]
        public static Gain PollutantGain(string name = "", Profile profile = null, GainUnit unit = GainUnit.Undefined, double value = 0.0)
        {
            PollutantGain properties = new PollutantGain
            {
                Name = name,
                Profile = profile,
                Unit = unit,
                Value = value,
            };

            return Gain(name, GainType.Pollutant, properties);
        }

        [Description("Returns an Environment Gain object with Sensible Equipment Gain properties")]
        [Input("name", "The name of the sensible equipment gain, default empty string")]
        [Input("profile", "The profile to be used for this gain, default null")]
        [Input("unit", "The unit type of the gain from the Gain Unit enum, default undefined")]
        [Input("value", "The value the gain should provide to the space, default 0.0")]
        [Input("radiantFraction", "The fraction of radiance from this sensible equipment gain, default 0.0")]
        [Input("viewCoefficient", "The view coefficient of this sensible equipment gain, default 0.0")]
        [Output("sensibleEquipmentGain", "An Environment Gain object")]
        public static Gain SensibleEquipmentGain(string name = "", Profile profile = null, GainUnit unit = GainUnit.Undefined, double value = 0.0, double radiantFraction = 0.0, double viewCoefficient = 0.0)
        {
            SensibleEquipmentGain properties = new SensibleEquipmentGain
            {
                Name = name,
                Profile = profile,
                Unit = unit,
                Value = value,
                RadiantFraction = radiantFraction,
                ViewCoefficient = viewCoefficient,
            };

            return Gain(name, GainType.Equipment, properties);
        }*/
    }
}
