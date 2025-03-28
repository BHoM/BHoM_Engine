/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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

using BH.oM.Geometry;
using System.Linq;
using System.Collections.Generic;

using BH.oM.Environment.Elements;

using BH.Engine.Geometry;

using BH.oM.Base.Attributes;
using System.ComponentModel;

using BH.Engine.Base;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        [Description("Returns a collection of Environment Panels that have been split if any of them overlap each other to ensure no panels overlap")]
        [Input("panels", "A collection of Environment Panels to split")]
        [Output("panels", "A collection of Environment Panels that do not overlap")]
        public static List<Panel> SplitPanelsByOverlap(this List<Panel> panels)
        {
            List<Panel> rtnElements = new List<Panel>();
            List<Panel> oriElements = new List<Panel>(panels.Select(x => x.DeepClone<Panel>()).ToList());

            while (oriElements.Count > 0)
            {
                Panel currentElement = oriElements[0];
                List<Panel> overlaps = currentElement.IdentifyOverlaps(oriElements);
                overlaps.AddRange(currentElement.IdentifyOverlaps(rtnElements));

                if (overlaps.Count == 0)
                    rtnElements.Add(currentElement);
                else
                {
                    //Cut the smaller building element out of the bigger one as an opening
                    List<Line> cuttingLines = new List<Line>();
                    foreach(Panel be in overlaps)
                        cuttingLines.AddRange(be.Polyline().SubParts());


                    rtnElements.AddRange(currentElement.Split(cuttingLines));
                }

                oriElements.RemoveAt(0);
            }

            return rtnElements;
        }
    }
}





