/*
 * This file is part of the Buildings and Habitats object Model(BHoM)
 * Copyright(c) 2015 - 2019, the respective contributors.All rights reserved.

*
* Each contributor holds copyright over their respective contributions.

* The project versioning(Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or
* (at your option) any later version.

*
* The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License
 * along with this code.If not, see<https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environment;
using BH.oM.Environment.Elements;
using BH.oM.Environment.Properties;
using BH.oM.Base;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("BH.Engine.Environment.Query.ExposedToSun => Defines whether an Environment Panel is exposed to the Sun or not")]
        [Input("panel", "An Environment Panel")]
        [Output("True if the panel is on the exterior of the model and has the potential to be exposed to the sun, false otherwise")]
        public static bool ExposedToSun(this Panel panel)
        {
            return (panel.Type == PanelType.Roof || panel.Type == PanelType.WallExternal);
        }
    }
}
