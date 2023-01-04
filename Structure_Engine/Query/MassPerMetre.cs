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

using BH.oM.Structure.Elements;
using BH.oM.Structure.SectionProperties;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Calculates the mass per length for the section as its area mulitplied by the density.")]
        [Input("section", "The SectionProperty to calculate the mass per area for.")]
        [Output("massPerLength", "The mass per length for the section.", typeof(MassPerUnitLength))]
        public static double MassPerMetre(this ISectionProperty section)
        {
            return section.IsNull() ? 0 : section.Area * section.Material.Density;
        }

        /***************************************************/

        [Description("Calculates the mass per length for the section as its area times density. Does not take any reinforcement into acount.")]
        [Input("section", "The ConcreteSection to calculate the mass per area for.")]
        [Output("massPerLength", "The mass per length for the section.", typeof(MassPerUnitLength))]
        public static double MassPerMetre(this ConcreteSection section)
        {
            //TODO: Handle reinforcement
            return section.IsNull() ? 0 : section.IsNull() ? 0 : section.Area * section.Material.Density;
        }

        /***************************************************/

        [Description("Calculates the mass per length for the section the mass per metre of the concrete section + the mass per metre of the steel section. Does not take any reinforcement into acount.")]
        [Input("section", "The CompositeSection to calculate the mass per area for.")]
        [Output("massPerLength", "The mass per length for the section.", typeof(MassPerUnitLength))]
        public static double MassPerMetre(this CompositeSection section)
        {
            //TODO: Handle embedment etc..
            return section.IsNull() ? 0 : section.ConcreteSection.MassPerMetre() + section.SteelSection.MassPerMetre();
        }

        /***************************************************/

        [Description("Calculates the mass per length for the section as its area mulitplied by the density.")]
        [Input("section", "The CableSection to calculate the mass per area for.")]
        [Output("massPerLength", "The mass per length for the section.", typeof(MassPerUnitLength))]
        public static double MassPerMetre(this CableSection section)
        {
            //TODO: Add property for kg/m as part of the cable section?
            return section.IsNull() ? 0 : section.Area * section.Material.Density;
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Calculates the mass per length for the section, generally as its area mulitplied by the density. General dispatch method that calls the correct method based on type.")]
        [Input("section", "The SectionProperty to calculate the mass per area for.")]
        [Output("massPerLength", "The mass per length for the section.", typeof(MassPerUnitLength))]
        public static double IMassPerMetre(this ISectionProperty section)
        {
            return section.IsNull() ? 0 : MassPerMetre(section as dynamic);
        }

        /***************************************************/
    }
}




