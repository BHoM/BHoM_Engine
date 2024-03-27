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

using BH.oM.Dimensional;
using BH.oM.Geometry;
using BH.oM.Environment.Elements;
using System.Collections.Generic;
using System.Linq;

using BH.oM.Base.Attributes;
using System.ComponentModel;

using BH.Engine.Base;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/

        [Description("BH.Engine.Environment.Modify.SetInternalElements2D => Assign a new collection of internal 2D elements to an Environment Panel")]
        [Input("panel", "An Environment Panel to update")]
        [Input("internalElements2D", "A collection of internal 2D elements to assign to the panel")]
        [Output("panel", "The updated Environment Panel")]
        public static Panel SetInternalElements2D(this Panel panel, List<IElement2D> internalElements2D)
        {
            if(panel == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot set the internal 2D elements of a null panel.");
                return panel;
            }
            
            if(internalElements2D == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot set null internal 2D elements to a panel.");
                return panel;
            }

            Panel pp = panel.ShallowClone();
            pp.Openings = new List<Opening>(internalElements2D.Cast<Opening>().ToList());
            return pp;
        }

        /***************************************************/

    }
}





