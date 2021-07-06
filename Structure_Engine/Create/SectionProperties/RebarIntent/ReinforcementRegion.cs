/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using BH.oM.Structure.SectionProperties.Reinforcement;
using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;
using BH.Engine.Geometry;


namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a ReinforcementRegion using a perimeter curve and an orientation for use with PanelReinforcement.")]
        [Input("perimeter", "The curve defining the region within the bounds of the Panel.")]
        [Input("orientation", "The orientation used to ditctae the directions of the reinforcement.\n" +
            "Longitudinal follows the x axis and transverse follows the y axis.")]
        [Output("region", "The ReinforcementRegion to be used with PanelReinforcement.")]
        public static ReinforcementRegion ReinforcementRegion(ICurve perimeter, Basis orientation)
        {
            if (perimeter.IsNull() || orientation.IsNull())
                return null;

            return new ReinforcementRegion() { Perimeter = perimeter, Orientation = orientation };
        }

        /***************************************************/
    }
}


