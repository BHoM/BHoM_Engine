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

using System.Linq;
using System.Collections.Generic;
using BH.oM.Environment.Elements;

using BH.oM.Geometry;
using BH.Engine.Geometry;

using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Merges the properties two Environment Panels together and returns a copied panel with the smallest area.")]
        [Input("panel1", "An Environment Panel to merge from.")]
        [Input("panel2", "A second Environment Panel to merge from.")]
        [Input("takeSmallestArea", "Defines whether to take the panel geometry with the smallest area, or the panel geometry with the largest geometry.")]
        [Output("mergedPanel", "The Environment Panel with the chosen area of the two provided but with the combined properties of both.")]
        public static Panel MergePanels(this Panel panel1, Panel panel2, bool takeSmallestArea = true)
        {
            if(panel1 == null || panel2 == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot merge null panels.");
                return null;
            }

            Panel rtnPanel = null;

            if (takeSmallestArea)
            {
                if (panel1.Area() > panel2.Area())
                    rtnPanel = panel2.Copy();
                else
                    rtnPanel = panel1.Copy();
            }
            else
            {
                if (panel1.Area() > panel2.Area())
                    rtnPanel = panel1.Copy();
                else
                    rtnPanel = panel2.Copy();
            }

            List<string> connectedSpaces = panel1.ConnectedSpaces;
            connectedSpaces.AddRange(panel2.ConnectedSpaces);
            connectedSpaces = connectedSpaces.Where(x => !x.Equals("-1")).ToList();
            connectedSpaces = connectedSpaces.Distinct().ToList();

            rtnPanel.ConnectedSpaces = new List<string>(connectedSpaces);
            
            return rtnPanel;
        }

        [Description("Merges the geometry of two Environment Panels if they share same connected space together and get panel with bigger area, tries boolean geometry and returns panel with new geometry.")]
        [Input("panel1", "An Environment Panel to merge from.")]
        [Input("panel2", "A second Environment Panel to merge from.")]
        [Output("mergedPanel", "The Environment Panel with the  merged area between two panels, but with properties of larger panel.")]
        public static Panel MergePanels(this Panel panel1, Panel panel2, double tolerance = Tolerance.Distance)
        {
            Panel rtnPanel = null;

            if (panel1.Area() > panel2.Area())
                rtnPanel = panel1.Copy();
            else
                rtnPanel = panel2.Copy();

            List<string> connectedSpaces = panel1.ConnectedSpaces;
            connectedSpaces.AddRange(panel2.ConnectedSpaces);
            connectedSpaces = connectedSpaces.Where(x => !x.Equals("-1")).ToList();
            connectedSpaces = connectedSpaces.Distinct().ToList();

            var result = connectedSpaces.All(panel2.ConnectedSpaces.Contains) && connectedSpaces.Count == panel2.ConnectedSpaces.Count;

            if (result)
            {
                rtnPanel.ConnectedSpaces = new List<string>(connectedSpaces);

                List<Panel> panels = new List<Panel>();
                panels.Add(panel1);
                panels.Add(panel2);

                List<Polyline> pLines = panels.Select(x => x.Polyline()).ToList();
                List<Polyline> newGeometry = pLines.BooleanUnion(tolerance);

                if (newGeometry.Count < 1)
                {
                    Base.Compute.RecordError("Could not output geometry for panel");
                    return null;
                }

                Polyline max = newGeometry[0];
                foreach (Polyline pl in newGeometry)
                {
                    if (pl.Area() > max.Area())
                        max = pl;
                }

                rtnPanel = rtnPanel.SetGeometry(max);

                return rtnPanel;
            }

            Base.Compute.RecordError("These panels do not belong to the same space(s) and cannot be merged together");
            return null;

        }
    }
}






