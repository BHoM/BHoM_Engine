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

using System.Linq;
using System.Collections.Generic;
using BH.oM.Environment.Elements;
using BH.oM.Geometry;
using BH.Engine.Geometry;

using BH.oM.Base.Attributes;
using System.ComponentModel;
using BH.Engine.Base;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a single Environment Panel with the provided opening. Opening is added to the provided panel regardless of geometric association")]
        [Input("panel", "A single Environment Panel to add the opening to")]
        [Input("opening", "The Environment Opening to add to the panel")]
        [Output("panel", "A modified Environment Panel with the provided opening added")]
        public static Panel AddOpening(this Panel panel, Opening opening)
        {
            if(panel == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot add an opening to a null panel.");
                return panel;
            }

            if(opening == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot add a null opening to a panel.");
                return panel;
            }

            Panel clone = panel.DeepClone<Panel>();
            if (clone.Openings == null) clone.Openings = new List<Opening>();
            clone.Openings.Add(opening);
            return clone;
        }

        [Description("Returns a list of Environment Panel with the provided openings added. Openings are added to the panels which contain them geometrically.")]
        [Input("panels", "A collection of Environment Panels to add the opening to")]
        [Input("openings", "A collection of Environment Openings to add to the panels")]
        [Input("centroidTolerance", "Set the tolerance for obtaining the centroid of openings, default is set to BH.oM.Geometry.Tolerance.Distance")]
        [Input("containingTolerance", "Set the tolerance for determining geometric association of openings to panels, default is set to BH.oM.Geometry.Tolerance.Distance")]
        [Output("panels", "A collection of modified Environment Panels with the provided openings added")]
        public static List<Panel> AddOpenings(this List<Panel> panels, List<Opening> openings, double centroidTolerance = BH.oM.Geometry.Tolerance.Distance, double containingTolerance = BH.oM.Geometry.Tolerance.Distance)
        {
            if (panels == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot add openings to null panels.");
                return panels;
            }

            if (openings == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot add null openings to panels.");
                return panels;
            }

            List<Panel> clonedPanels = new List<Panel>(panels.Select(x => x.DeepClone<Panel>()).ToList());
            List<Opening> clonedOpenings = new List<Opening>(openings.Select(x => x.DeepClone<Opening>()).ToList());

            foreach (Opening o in clonedOpenings)
            {
                Point centre = o.Polyline().Centroid(centroidTolerance);
                if (centre != null)
                {
                    Panel panel = clonedPanels.PanelsContainingPoint(centre, false, containingTolerance).Item1.FirstOrDefault();
                    if (panel != null)
                    {
                        if (panel.Openings == null) panel.Openings = new List<Opening>();
                        panel.Openings.Add(o);
                    }
                }
            }

            return clonedPanels;
        }
    }
}




