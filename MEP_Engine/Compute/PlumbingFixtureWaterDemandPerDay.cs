/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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

using System.ComponentModel;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.MEP.Fixtures;
using BH.oM.Architecture.Elements;
using BH.Engine.Base;

namespace BH.Engine.MEP
{
    public static partial class Compute
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("Calculates the plumbing fixture water demand per day using the plumbing fixture flows and the building occupancy.")]
        [Input("occupancy", "The BHoM Object which contains the building's occupancy by gender. This method requires the object's occupancy percentages to be set.")]
        [Input("fixtureFlow", "A fixtureFlow object that describes fixture usage in volume and duration.")]
        [Input("fixtureUsage", "A commercial or residential fixtureUsage object that describes occupancy-specific usage per fixture type.")]
        [Output("waterPerDay", "The amount of water in m^3 used on a daily basis by plumbing fixtures.")]
        public static double PlumbingFixtureWaterDemandPerDay(Occupancy occupancy, IFixtureFlow fixtureFlow, IFixtureUsage fixtureUsage)
        {
            if(occupancy == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the plumbing fixture water demand from a null occupancy object.");
                return -1;
            }

            if(fixtureFlow == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the plumbing fixture water demand from a null fixture flow object.");
                return -1;
            }

            if(fixtureUsage == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the plumbing fixture water demand from a null fixture usage object.");
                return -1;
            }

            double numberFemales = occupancy.OccupantCount * occupancy.FemalePercentage;
            double numberMales = occupancy.OccupantCount * occupancy.MalePercentage;
            double numberGenderNeutral = occupancy.OccupantCount * occupancy.GenderNeutralPercentage;

            double toiletFlow = System.Convert.ToDouble(fixtureFlow.PropertyValue("ToiletVolumePerUse"));
            double lavatoryFlow = System.Convert.ToDouble(fixtureFlow.PropertyValue("UrinalVolumePerUse"));
            double urinalFlow = System.Convert.ToDouble(fixtureFlow.PropertyValue("LavatoryVolumePerUse"));
            double showerFlow = System.Convert.ToDouble(fixtureFlow.PropertyValue("ShowerVolumePerUse"));
            double kitchenFaucetFlow = System.Convert.ToDouble(fixtureFlow.PropertyValue("KitchenFaucetVolumePerUse"));

            double toiletNumberOfUsesMale = System.Convert.ToDouble(fixtureUsage.PropertyValue("ToiletNumberOfUsesMale"));
            double toiletNumberOfUsesFemale = System.Convert.ToDouble(fixtureUsage.PropertyValue("ToiletNumberOfUsesFemale"));
            double toiletNumberOfUsesGenderNeutral = System.Convert.ToDouble(fixtureUsage.PropertyValue("ToiletNumberOfUsesGenderNeutral"));
            double showerNumberOfUsesMale = System.Convert.ToDouble(fixtureUsage.PropertyValue("ShowerNumberOfUsesMale"));
            double showerNumberOfUsesFemale = System.Convert.ToDouble(fixtureUsage.PropertyValue("ShowerNumberOfUsesFemale"));
            double showerNumberOfUsesGenderNeutral = System.Convert.ToDouble(fixtureUsage.PropertyValue("ShowerNumberOfUsesGenderNeutral"));
            double kitchenFaucetNumberOfUsesMale = System.Convert.ToDouble(fixtureUsage.PropertyValue("KitchenFaucetNumberOfUsesMale"));
            double kitchenFaucetNumberOfUsesFemale = System.Convert.ToDouble(fixtureUsage.PropertyValue("KitchenFaucetNumberOfUsesFemale"));
            double kitchenFaucetNumberOfUsesGenderNeutral = System.Convert.ToDouble(fixtureUsage.PropertyValue("KitchenFaucetNumberOfUsesGenderNeutral"));
            double lavatoryNumberOfUsesMale = System.Convert.ToDouble(fixtureUsage.PropertyValue("LavatoryNumberOfUsesMale"));
            double lavatoryNumberOfUsesFemale = System.Convert.ToDouble(fixtureUsage.PropertyValue("LavatoryNumberOfUsesFemale"));
            double lavatoryNumberOfUsesGenderNeutral = System.Convert.ToDouble(fixtureUsage.PropertyValue("LavatoryNumberOfUsesGenderNeutral"));
            double urinalNumberOfUsesMale = System.Convert.ToDouble(fixtureUsage.PropertyValue("UrinalNumberOfUsesMale"));

            double totaldailytoiletflow = (numberMales * toiletNumberOfUsesMale * toiletFlow) + (numberFemales * toiletNumberOfUsesFemale * toiletFlow) + (numberGenderNeutral * toiletNumberOfUsesGenderNeutral * toiletFlow);
            double totaldailylavatoryflow = (numberMales * lavatoryNumberOfUsesMale * lavatoryFlow) + (numberFemales * lavatoryNumberOfUsesFemale * lavatoryFlow) + (numberGenderNeutral * lavatoryNumberOfUsesGenderNeutral * lavatoryFlow);
            double totaldailykitchenfaucetflow = (numberMales * kitchenFaucetNumberOfUsesMale * kitchenFaucetFlow) + (numberFemales * kitchenFaucetNumberOfUsesFemale * kitchenFaucetFlow) + (numberGenderNeutral * kitchenFaucetNumberOfUsesGenderNeutral * kitchenFaucetFlow);
            double totaldailyshowerflow = (numberMales * showerNumberOfUsesMale * showerFlow) + (numberFemales * showerNumberOfUsesFemale * showerFlow) + (numberGenderNeutral * showerNumberOfUsesGenderNeutral * showerFlow);
            double totaldailyurinalflow = (numberMales * urinalNumberOfUsesMale * urinalFlow);
            double totaldailyplumbingfixturewaterdemand = totaldailytoiletflow + totaldailykitchenfaucetflow + totaldailylavatoryflow + totaldailyshowerflow + totaldailyurinalflow;

            return totaldailyplumbingfixturewaterdemand;
        }

        /***************************************************/

    }
}


