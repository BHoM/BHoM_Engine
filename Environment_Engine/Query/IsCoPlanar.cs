/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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

using BH.oM.Environment.Elements;
using System;
using System.Collections.Generic;
using System.Linq;

using BH.oM.Geometry;
using BH.Engine.Geometry;

using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Determines whether the two Environment Panels are co-planar")]
        [Input("panel", "An Environment Panel")]
        [Input("panelToCompare", "An Environment Panel to compare against")]
        [Output("isCoPlanar", "True if the two panels are co-planar, false otherwise")]
        public static bool IsCoPlanar(this Panel panel, Panel panelToCompare)
        {
            if(panel == null || panelToCompare == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query whether two panels are co planar if either panel is null.");
                return false;
            }

            return panel.Polyline().IsCoplanar(panelToCompare.Polyline());
        }
    }
}




