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
using BH.oM.Dimensional;
using System;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Facade.Elements;
using BH.oM.Base;
using BH.Engine.Geometry;
using BH.Engine.Spatial;
using BH.Engine.Base;
using BH.oM.Facade.Fragments;
using BH.oM.Facade.Results;

using BH.oM.Base.Attributes;
using System.ComponentModel;
using BH.oM.Facade.SectionProperties;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Facade
{
    public static partial class Compute
    {
        /***************************************************/
        /****          Public Methods                   ****/
        /***************************************************/

        [Description("Returns effective U - Value of a panel calculated using the Single Assessment Method(Using Psi-tj). Requires center of opening U-value and frame Psi-tj value as OpeningConstruction and FrameEdgeProperty fragments on any Openings within the Panel.")]
        [Input("panel", "Panel to find U-value for.")]
        [Output("effectiveUValue", "Effective U-value result of panel calculated using SAM.")]
        public static OverallUValue UValuePanelSAM(this Panel panel)
        {
            if (panel == null)
            {
                Base.Compute.RecordError($"U-Value can not be calculated for null panel.");
                return null;
            }

            double panelArea = panel.Area();
            if (panelArea == 0)
            {
                BH.Engine.Base.Compute.RecordError($"Panel {panel.BHoM_Guid} has a calculated area of 0. Ensure the panel is valid with associated edges defining its geometry and try again.");
                return null;
            }

            double panelUValue = panel.PanelEffectiveUValue();
            if (panelUValue == double.NaN)
                return null;

            Base.Compute.RecordNote("Panels assessed using SAM method will use SAM method for any contained Openings, but will only assess the Panel itself based on its assigned Continuous and/or Cavity U Values.");
            double effectiveUValue = panelUValue;
            List<Opening> panelOpenings = panel.Openings;
            if (panelOpenings.Count > 0)
            {
                double uValueProduct = panelUValue * panelArea;
                double totalArea = panelArea;
                foreach (Opening opening in panelOpenings)
                {
                    double area = opening.Area();
                    uValueProduct += opening.UValueOpeningSAM().UValue * area;
                    totalArea += area;
                }
                if (totalArea == 0)
                {
                    Base.Compute.RecordError("Openings have a total calculated area of 0. Ensure Openings are valid with associated edges defining their geometry and try again.");
                    return null;
                }
                effectiveUValue = uValueProduct / totalArea;
            }

            OverallUValue result = new OverallUValue(effectiveUValue, new List<IComparable> { panel.BHoM_Guid });
            return result;
        }

        /***************************************************/

    }
}


