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

using System;
using System.Collections.Generic;

using System.Linq;
using BH.oM.Environment.Elements;

using BH.Engine.Geometry;
using BH.oM.Geometry;

using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Removes panels which overlap each other")]
        [Input("panels", "A collection of Environment Panels")]
        [Output("panels", "A collection of Environment Panels with no overlaps")]
        public static List<Panel> CullOverlaps(this List<Panel> panels)
        {
            List<Panel> ori = new List<Panel>(panels);
            List<Panel> toReturn = new List<Panel>();

            while (ori.Count > 0)
            {
                Panel current = ori[0];
                List<Panel> overlaps = current.IdentifyOverlaps(panels);

                foreach (Panel be in overlaps)
                    ori.Remove(be);

                toReturn.Add(current);
                ori.RemoveAt(0);
            }

            return toReturn;
        }
    }
}




