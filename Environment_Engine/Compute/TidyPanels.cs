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
using System.ComponentModel;

using BH.oM.Environment.Elements;
using BH.oM.Base.Attributes;

namespace BH.Engine.Environment
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a list of Environment Panels with overlapping panels split and merged for non-horizontal panels.")]
        [Input("panels", "A collection of Environment Panels to tidy.")]
        [Input("distanceTolerance", "Distance tolerance for calculating discontinuity points, default is set to the value defined by BH.oM.Geometry.Tolerance.Distance.")]
        [Input("angleTolerance", "Angle tolerance for calculating discontinuity points, default is set to the value defined by BH.oM.Geometry.Tolerance.Angle.")]
        [Input("numericTolerance", "Tolerance for determining whether a calulated number is within a range defined by the tolerance, default is set to the value defined by BH.oM.Geometry.Tolerance.Distance.")]
        [Input("autoFixPanelOrientations", "Whether or not the panels should be flipped away from space")]
        [Output("panels", "A collection of modified Environment Panels with overlapping panels split and merged.")]
        public static List<Panel> TidyPanels(this List<Panel> panels, bool autoFixPanelOrientations = true, double distanceTolerance = BH.oM.Geometry.Tolerance.Distance, double angleTolerance = BH.oM.Geometry.Tolerance.Angle, double numericTolerance = BH.oM.Geometry.Tolerance.Distance)
        {
            if (panels == null)
                return panels;
            
            List<Panel> flipPanels = panels.Where(x =>
            {
                double tilt = x.Tilt(distanceTolerance, angleTolerance);
                return (tilt >= 0 - numericTolerance && tilt <= 0 + numericTolerance) || (tilt >= 180 - numericTolerance && tilt <= 180 + numericTolerance);
            }).ToList();

            List<Panel> returnPanels = flipPanels;

            panels = panels.Where(x =>
            {
                double tilt = x.Tilt(distanceTolerance, angleTolerance);
                return !((tilt >= 0 - numericTolerance && tilt <= 0 + numericTolerance) || (tilt >= 180 - numericTolerance && tilt <= 180 + numericTolerance));
            }).ToList();

            List<Panel> fixedPanels = new List<Panel>();
            List<Panel> splitPanels = panels.SplitPanelsByOverlap();
            List<List<Panel>> overlappingPanels = splitPanels.Select(x => x.IdentifyOverlaps(splitPanels)).ToList();
            List<Guid> handledPanels = new List<Guid>();

            for (int x = 0; x < splitPanels.Count; x++)
            {
                if (handledPanels.Contains(splitPanels[x].BHoM_Guid))
                    continue; //This panel has already been handled
                
                if (overlappingPanels[x].Count == 0)
                {
                    fixedPanels.Add(splitPanels[x]);
                    handledPanels.Add(splitPanels[x].BHoM_Guid);
                    continue;
                }

                Panel p = splitPanels[x];
                for (int y = 0; y < overlappingPanels[x].Count; y++)
                {
                    p = p.MergePanels(overlappingPanels[x][y], false);
                    handledPanels.Add(overlappingPanels[x][y].BHoM_Guid);
                }

                fixedPanels.Add(p);
                handledPanels.Add(p.BHoM_Guid);
            }

            if (autoFixPanelOrientations)
            {
                List<Panel> cullDuplicates = new List<Panel>();
                flipPanels.AddRange(fixedPanels);
                List<List<Panel>> panelsAsSpaces = flipPanels.ToSpaces();
                for (int i = 0; i < panelsAsSpaces.Count; i++)
                {
                    panelsAsSpaces[i].FlipPanels();
                    cullDuplicates.AddRange(panelsAsSpaces[i]);
                }

                cullDuplicates = cullDuplicates.Where(x =>
                    {
                        double tilt = x.Tilt(distanceTolerance, angleTolerance);
                        return (tilt >= 0 - numericTolerance && tilt <= 0 + numericTolerance) || (tilt >= 180 - numericTolerance && tilt <= 180 + numericTolerance);
                    }).ToList();

                List<Panel> culledPanels = cullDuplicates.CullDuplicates();
                foreach (Panel p in culledPanels)
                    returnPanels.Add(p);

                returnPanels = returnPanels.CullDuplicates();
            }
            else
                returnPanels.AddRange(fixedPanels);

            return returnPanels;
        }
    }
}


