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

        [Description("Returns a collection of Environment Panels that are split by their edge points ensuring panels which span multiple spaces are split and have adjacencies assigned correctly")]
        [Input("panels", "A collection of Environment Panels to split")]
        [Output("panels", "A collection of Environment Panels that have been split")]
        public static List<Panel> SplitPanelsByPoints(this List<Panel> panels)
        {
            List<Panel> rtnElements = new List<Panel>();
            List<Panel> oriElements = new List<Panel>(panels.Select(x => x.DeepClone<Panel>()).ToList());

            while (oriElements.Count > 0)
            {
                Panel currentElement = oriElements[0];

                bool wasSplit = false;
                List<Panel> elementSplitResult = new List<Panel>();
                for (int x = 0; x < oriElements.Count; x++)
                {
                    if (oriElements[x].BHoM_Guid == currentElement.BHoM_Guid) continue; //Don't split by the same element

                    //Split this element by each other element in the list
                    elementSplitResult = currentElement.Split(oriElements[x]);

                    if (elementSplitResult.Count > 1)
                    {
                        oriElements.AddRange(elementSplitResult);
                        wasSplit = true;
                        break; //Don't attempt to split this element any further, wait to split its new parts later in the loop...
                    }
                }

                oriElements.RemoveAt(0); //Remove the element we have just worked with, regardless of whether we split it or not
                if (!wasSplit)
                    rtnElements.Add(currentElement); //We have a pure element ready to use
                else
                {
                    //Add the new elements to the list of cutting objects
                    oriElements.RemoveAt(oriElements.IndexOf(oriElements.Where(x => x.BHoM_Guid == currentElement.BHoM_Guid).FirstOrDefault()));
                    oriElements.AddRange(elementSplitResult);
                }
            }

            return rtnElements;
        }
    }
}




