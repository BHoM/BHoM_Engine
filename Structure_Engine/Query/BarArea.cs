/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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

using BH.Engine.Geometry;
using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using System.Collections.Generic;
using System.Linq;
using System;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using BH.oM.Structure.Reinforcement;
using System.ComponentModel;
using BH.Engine.Analytical;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Calculates the area of a single rebar in the IBarReinforcement. To get the total reinforcement area for the reinforcement layout please call the Area method.")]
        [Input("reinforcement", "The LongitudinalReinforcement to calculate the area for.")]
        [Output("area", "The area of a single rebar the IBarReinforcement.", typeof(Area))]
        public static double BarArea(this IBarReinforcement reinforcement)
        {
            return reinforcement.Diameter * reinforcement.Diameter / 4 * Math.PI;
        }

        /***************************************************/

        [Description("Calculates the area of a single rebar in the PanelReinforcement in the logitudinal direction.")]
        [Input("reinforcement", "The PanelReinforcement to calculate the area for.")]
        [Output("area", "The area of a single rebar in the logitudinal direction the PanelReinforcement.", typeof(Area))]
        public static double LongitudinalBarArea(this PanelReinforcement reinforcement)
        {
            return reinforcement.LongitudinalDiameter * reinforcement.LongitudinalDiameter / 4 * Math.PI;
        }

        /***************************************************/

        [Description("Calculates the area of a single rebar in the PanelReinforcement in the logitudinal direction.")]
        [Input("reinforcement", "The PanelReinforcement to calculate the area for.")]
        [Output("area", "The area of a single rebar in the transversal direction the PanelReinforcement.", typeof(Area))]
        public static double TransversalBarArea(this PanelReinforcement reinforcement)
        {
            return reinforcement.TransverseDiameter * reinforcement.TransverseDiameter / 4 * Math.PI;
        }

        /***************************************************/

    }

}







