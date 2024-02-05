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

using BH.Engine.Geometry;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Environment.Configuration;
using BH.oM.Environment.Elements;
using BH.oM.Geometry;
using BH.oM.Quantities.Attributes;
using ICSharpCode.Decompiler.IL.Patterns;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using static Humanizer.In;

namespace BH.Engine.Environment
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public methods                            ****/
        /***************************************************/

        [Description("Creates openings for input Panels at the locations provided.\nThe total area of the openings is equal to the total area of the external panels, multiplied by the glazing ratio.")]
        [Input("panels", "A collection of Environment Panels which will be used to identify the host panel for the opening from the provided location point.")]
        [Input("openingLocations", "The point in 3D space corresponding to the desired locations of the openings.")]
        [Input("glazingRatio", "Ratio of total external panel surface area and glazed area. Ratio as decimal {0-1}.")]
        [Input("panelsToIgnore", "Optional input for selecting a collection of Environment Panels to be ignored in the calculation")]
        [Input("sillHeight", "Optional input for defining the distance between the base of the panel and the bottom of the opening - default: 0.5.")]
        [Input("openingHeight", "Optional input for defining the height of opening - default: 1.2.")]
        [Input("openingType", "Optional input for defining the opening type of the output Openings, can be either 'Glazing' or 'Door' - default: Glazing.")]
        [Output("openings", "Returns the constructed openings which can then be applied to panels.")]
        public static List<Opening> OpeningsFromGlazingRatio(
            List<Panel> panels,
            List<Point> openingLocations,
            double glazingRatio,
            List<Panel> panelsToIgnore = null,
            double sillHeight = 0.5,
            double openingHeight = 1.2,
            OpeningType openingType = OpeningType.Glazing
            ) 
        {
            switch (openingType)
            {
                case OpeningType.Door:
                case OpeningType.Glazing:
                    break;
                default:
                    BH.Engine.Base.Compute.RecordError($"Only openings of type 'Glazing' and 'Door' are supported.");
                    return new List<Opening>();
            }
            List<Panel> externalPanels = panels.IsExternal().Item1;
            List<Panel> filteredPanels = externalPanels.RemovePanels(panelsToIgnore ?? new List<Panel>());

            double totalArea = filteredPanels.Select(x => x.Area()).Sum();
            double existingOpeningArea = filteredPanels.Select(x => x.Openings.Select(y => y.Polyline().Area()).Sum()).Sum();

            //area available for glazing is total area * glazing ratio
            //area avaialable per for new glazing is area avaialble minus area used by other galzing 
            //area per window avaialable is total area left divided for number of windows
            //opening width is then area per window divided by the entered height
            double openingWidth = ((totalArea * glazingRatio - existingOpeningArea) / openingLocations.Count) / openingHeight;

            OpeningOption option = new OpeningOption()
            {
                Height = openingHeight,
                Width = openingWidth,
                SillHeight = sillHeight,
                Type = openingType,
            };

            List<Opening> openings = new List<Opening>();

            foreach(Point point in openingLocations)
            {
                Opening opening = Create.Opening(point, option, filteredPanels);

                if(opening != null)
                    openings.Add(opening);
                else
                    BH.Engine.Base.Compute.RecordWarning($"Could not find a host panel for the opening at ({point.X}, {point.Y}, {point.Z}). An Opening has not been created for this location.");
            }

            return openings;
        }

        /***************************************************/
    }
}