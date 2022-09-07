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

namespace BH.Engine.Facade
{
    public static partial class Compute
    {
        /***************************************************/
        /****          Public Methods                   ****/
        /***************************************************/

        [Description("Returns effective U-Value of panel calculated using the Area Weighting Method. Requires continuous U-value as Construction fragment.")]
        [Input("panel", "Panel to find U-value for.")]
        [Output("effectiveUValue", "Effective U-value result of panel calculated using area weighting.")]
        public static OverallUValue UValuePanelAW(this Panel panel)
        {
            if (panel == null)
            {
                Base.Compute.RecordError($"U-Value can not be calculated for null panel.");
                return null;
            }

            List<IFragment> glassUValues = panel.Construction.GetAllFragments(typeof(UValueGlassCentre));
            if (glassUValues.Count > 0)
            {
                BH.Engine.Base.Compute.RecordError($"Panel {panel.BHoM_Guid} has Glass U-value assigned. Panels can only receive Continuous U-value");
                return null;
            }

            List<IFragment> glassEdgeUValues = panel.Construction.GetAllFragments(typeof(UValueGlassEdge));
            if (glassEdgeUValues.Count > 0)
            {
                BH.Engine.Base.Compute.RecordError($"Panel {panel.BHoM_Guid} has Glass edge U-value assigned. Panels can only receive Continuous U-value");
                return null;
            }

            List<IFragment> contUValues = panel.Construction.GetAllFragments(typeof(UValueContinuous));
            if (contUValues.Count <= 0)
            {
                Base.Compute.RecordError($"Panel {panel.BHoM_Guid} does not have Continuous U-value assigned.");
                return null;
            }
            if (contUValues.Count > 1)
            {
                Base.Compute.RecordError($"Panel {panel.BHoM_Guid} has more than one Continuous U-value assigned.");
                return null;
            }
            double contUValue = (contUValues[0] as UValueContinuous).UValue;

            List<FrameEdge> frameEdges = panel.ExternalEdges;
            List<double> frameUValues = new List<double>();
            for (int i = 0; i < frameEdges.Count; i++)
            {
                List<IFragment> f_uValues = frameEdges[i].FrameEdgeProperty.GetAllFragments(typeof(UValueFrame));
                if (f_uValues.Count > 0)
                {
                    BH.Engine.Base.Compute.RecordError($"Panel {panel.BHoM_Guid} has Frame U-value assigned. Frame U-values are not included in calculation.");
                    continue;
                }
            }

            double panelArea = panel.Area();
            if (panelArea == 0)
            {
                BH.Engine.Base.Compute.RecordError($"Panel {panel.BHoM_Guid} has a calculated area of 0. Ensure the panel is valid with associated edges defining its geometry and try again.");
            }
            double panelUValue = contUValue;
            double effectiveUValue = panelUValue;

            List<Opening> panelOpenings = panel.Openings;
            if (panelOpenings.Count > 0)
            {
                double uValueProduct = panelUValue;
                double totalArea = panelArea;
                foreach (Opening opening in panelOpenings)
                {
                    double area = opening.Area();
                    uValueProduct += opening.UValueOpeningAW().UValue * area;
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