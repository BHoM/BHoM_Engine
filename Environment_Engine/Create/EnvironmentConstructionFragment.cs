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

using BH.oM.Environment.Fragments;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Create
    {
        [Deprecated("3.0", "Deprecated to remove name input", null, "EnvironmentConstructionFragment(fFactor, additionalHeatTransfer)")]
        public static EnvironmentConstructionFragment EnvironmentConstructionFragment(string name = "", double fFactor = 0.0, double additionalHeatTransfer = 0.0)
        {
            return Create.EnvironmentConstructionFragment(fFactor, additionalHeatTransfer);
        }

        [Description("Returns an Environment Construction Fragment object")]
        [Input("fFactor", "The FFactor for the construction, default 0.0")]
        [Input("additionalHeatTransfer", "The additional heat transfer through the construction, default 0.0")]
        [Output("environmentConstructionFragment", "An Environment Construction Fragment object - this can be added to a Construction object")]
        [Deprecated("3.0", "Deprecated in favour of default create components produced by BHoM")]
        public static EnvironmentConstructionFragment EnvironmentConstructionFragment(double fFactor = 0.0, double additionalHeatTransfer = 0.0)
        {
            return new EnvironmentConstructionFragment
            {
                FFactor = fFactor,
                AdditionalHeatTransfer = additionalHeatTransfer,
            };
        }
    }
}
