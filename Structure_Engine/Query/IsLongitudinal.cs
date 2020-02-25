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

using BH.oM.Structure.SectionProperties.Reinforcement;
using BH.oM.Reflection.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Method returns whether the reinforcement is longitudinal or not. This is always true for LayerReinforcement.")]
        [Input("reinforcement", "The reinforcement object to check.")]
        [Output("result", "Returns true if the reinforcement is longitudinal. Always true for LayerReinforcement.")]
        public static bool IsLongitudinal(this LayerReinforcement reinforcement)
        {
            return true;
        }

        /***************************************************/

        [Description("Method returns whether the reinforcement is longitudinal or not. This is always true for PerimeterReinforcement.")]
        [Input("reinforcement", "The reinforcement object to check.")]
        [Output("result", "Returns true if the reinforcement is longitudinal. Always true for PerimeterReinforcement.")]
        public static bool IsLongitudinal(this PerimeterReinforcement reinforcement)
        {
            return true;
        }

        /***************************************************/

        [Description("Method returns whether the reinforcement is longitudinal or not. This is always false for TieReinforcement.")]
        [Input("reinforcement", "The reinforcement object to check.")]
        [Output("result", "Returns true if the reinforcement is longitudinal. Always false for TieReinforcement.")]
        public static bool IsLongitudinal(this TieReinforcement reinforcement)
        {
            return false;
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Method returns whether the reinforcement is longitudinal or not.")]
        [Input("reinforcement", "The reinforcement object to check.")]
        [Output("result", "Returns true if the reinforcement is longitudinal.")]
        public static bool IIsLongitudinal(this Reinforcement reinforcement)
        {
            return IsLongitudinal(reinforcement as dynamic);
        }

        /***************************************************/
    }
}

