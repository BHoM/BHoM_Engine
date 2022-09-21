/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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

using BH.Engine.Spatial;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Structure.Elements;
using BH.oM.Structure.SurfaceProperties;
using BH.oM.Geometry;
using BH.oM.Dimensional;
using System;
using System.Collections.Generic;
using System.Linq;
using BH.Engine.Geometry;
using BH.Engine.Base;
using System.ComponentModel;
using BH.Engine.Analytical;

namespace BH.Engine.Structure
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Merges a list of panels as the boolean union of all coplanar external outlines, with openings cut in after the fact. Properties are assigned per the guide panel")]
        [Input("panels", "Panels to merge.")]
        [Input("propertyNames", "Properties to group by - properties not mentioned will be ignored.")]
        [Output("panels", "The created Panel.")]
        public static List<Panel> MergePanels(List<Panel> panels, List<String> propertyNames, double tolerance = Tolerance.Distance)
        {
            HashSet<String> availableProperties = panels.First().GetAllPropertyFullNames();
            List<String> availablePropNames = availableProperties.Select(x => x.ToString().Split('.').Last()).ToList();
            propertyNames = propertyNames.Where(x => availablePropNames.Contains(x)).ToList();

            IEnumerable<IGrouping<string, Panel>> panelGroups = panels.GroupBy(x => x.HashProperties(propertyNames));

            List<Panel> outPanels = new List<Panel>();

            foreach (IGrouping<string, Panel> group in panelGroups)
            {
                Panel guide = new Panel();
                foreach(string propName in propertyNames)
                {
                    guide.SetPropertyValue(propName, group.First().PropertyValue(propName));
                }
                outPanels.AddRange(MergePanels(group.ToList(), guide, tolerance));
            }

            return outPanels;
        }

        public static string HashProperties(this BHoMObject obj, IEnumerable<string> propertyNames)
        {
            String hash = "";
            foreach(string propertyName in propertyNames)
            {
                hash += obj.PropertyValue(propertyName).GetHashCode().ToString();
            }

            return hash;
        }

        [Description("Merges a list of panels as the boolean union of all coplanar external outlines, with openings cut in after the fact. Properties are assigned per the guide panel")]
        [Input("panels", "Panels to merge.")]
        [Input("guide", "Panel with properties to apply to all panels the geometry of this panel is ignored.")]
        [Output("panels", "The created Panel.")]
        public static List<Panel> MergePanels(List<Panel> panels, Panel guide, double tolerance = Tolerance.Distance)
        {
            List<Panel> emptyPanels = MergePanels(panels, tolerance);

            List<Panel> fullPanels = new List<Panel>();

            foreach (Panel emptyPanel in emptyPanels)
            {
                Panel fullPanel = guide.ShallowClone();
                fullPanel.ExternalEdges = emptyPanel.ExternalEdges;
                fullPanel.Openings = emptyPanel.Openings;

                fullPanels.Add(fullPanel);
            }

            return fullPanels;
        }
            
        [Description("Merges a list of panels as the boolean union of all coplanar external outlines, with openings cut in after the fact. The resulting panel has no properties")]
        [Input("panels", "Panels to merge.")]
        [Output("panels", "The created Panel.")]
        public static List<Panel> MergePanels(List<Panel> panels, double tolerance = Tolerance.Distance)
        {
            //Boolean the external edges together into some number of outlines
            IEnumerable<PolyCurve> outlines = panels.Select(x => x.ExternalPolyCurve());
            List<PolyCurve> newOutlines = outlines.BooleanUnion(tolerance);

            //Combine all openings into one big list of opening curves
            List<Opening> openings = new List<Opening>();
            foreach (Panel panel in panels)
            {
                openings.AddRange(panel.Openings);
            }

            //Sort out which new outlines get which openings and add them. 
            List<Panel> newPanels = new List<Panel>();
            foreach (PolyCurve newOutline in newOutlines)
            {
                List<Opening> filteredOpenings = openings.Where(x => newOutline.IIsContaining(x.OutlineCurve(), true, tolerance)).ToList();
                newPanels.Add(Create.Panel(newOutline, filteredOpenings));
            }

            return newPanels;
        }
    }
}