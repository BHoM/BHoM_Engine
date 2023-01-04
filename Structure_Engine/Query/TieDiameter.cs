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

using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.SectionProperties.Reinforcement;
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

        [Description("Extracts the the diameter of the first tie reinforecment of a ConcreteSection. If no TieReinforcement is found, 0 will be returned.")]
        [Input("property", "The ConcreteSection to get TieReinforcement diameter from.")]
        [Output("dia", "The diameter of the first TieReinforcement of the ConcreteSection.", typeof(Length))]
        public static double TieDiameter(this ConcreteSection property)
        {
            if (property.IsNull())
                return 0;

            foreach (Reinforcement reo in property.RebarIntent.BarReinforcement)
            {
                if (reo is TieReinforcement)
                {
                    return reo.Diameter;
                }
            }
            return 0;
        }

        /***************************************************/
    }
}




