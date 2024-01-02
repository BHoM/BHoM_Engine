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

        [Description("Returns effective U-Value of opening calculated using the Area Weighting Method. Requires center of opening U-value, frame U-value and edge U-value as OpeningConstruction and FrameEdgeProperty fragments.")]
        [Input("opening", "Opening to find U-value for.")]
        [Output("effectiveUValue", "Effective U-value result of opening calculated using area weighting.")]
        public static OverallUValue UValueOpeningAW(this Opening opening)
        {
            if (opening == null)
            {
                Base.Compute.RecordError($"U-Value can not be calculated for null opening.");
                return null;
            }
            if (opening.Type == OpeningType.Hole)
            {
                Base.Compute.RecordError($"U-Value can not be calculated for opening type Hole.");
                return null;
            }
            if (opening.Type == OpeningType.CurtainWallSpandrel)
            {
                return opening.UValueSpandrelAW();
            }
            if (opening.Type != OpeningType.CurtainWallVision && opening.Type != OpeningType.Window)
            {
                Base.Compute.RecordWarning($"Opening does not have valid OpeningType assigned. U-value calculation methods for vision opening have been applied.");
            }


            List<IFragment> glassUValues = opening.OpeningConstruction.GetAllFragments(typeof(UValueGlassCentre));
            List<IFragment> glassEdgeUValues = opening.OpeningConstruction.GetAllFragments(typeof(UValueGlassEdge));
            List<IFragment> contUValues = opening.OpeningConstruction.GetAllFragments(typeof(UValueContinuous));
            List<IFragment> cavityUValues = opening.OpeningConstruction.GetAllFragments(typeof(UValueCavity));
            double contUValue = 0;
            double cavityUValue = 0;

            double glassUValue = 0;
            double glassEdgeUValue = 0;

            if ((glassUValues.Count <= 0) && (glassEdgeUValues.Count <= 0) && (contUValues.Count <= 0) && (cavityUValues.Count <= 0))
            {
                BH.Engine.Base.Compute.RecordError($"Opening {opening.BHoM_Guid} does not have Glass U-values, Continuous U-value, or Cavity U-value assigned.");
                return null;
            }
            if ((glassUValues.Count == 1) && (glassEdgeUValues.Count <= 0))
            {
                BH.Engine.Base.Compute.RecordError($"Opening {opening.BHoM_Guid} has center of Glass U-value without Edge of Glass u-values assigned.");
                return null;
            }
            if ((glassUValues.Count <= 0) && (glassEdgeUValues.Count == 1))
            {
                BH.Engine.Base.Compute.RecordError($"Opening {opening.BHoM_Guid} has Edge of Glass U-values without center of Glass u-value assigned.");
                return null;
            }
            if (contUValues.Count > 1)
            {
                Base.Compute.RecordError($"Opening {opening.BHoM_Guid} has more than one continuous U-value assigned.");
                return null;
            }
            if (cavityUValues.Count > 1)
            {
                Base.Compute.RecordError($"Opening {opening.BHoM_Guid} has more than one cavity U-value assigned.");
                return null;
            }
            if (glassUValues.Count > 1)
            {
                BH.Engine.Base.Compute.RecordError($"Opening {opening.BHoM_Guid} has more than one Glass U-value assigned.");
                return null;
            }
            if (glassEdgeUValues.Count > 1)
            {
                Base.Compute.RecordError($"Opening {opening.BHoM_Guid} has more than one Glass edge U-value assigned.");
                return null;
            }

            if (contUValues.Count == 1)
            {
                contUValue = (contUValues[0] as UValueContinuous).UValue;
            }
            if (cavityUValues.Count == 1)
            {
                cavityUValue = (cavityUValues[0] as UValueCavity).UValue;
            }
            if (glassUValues.Count == 1 || glassEdgeUValues.Count == 1)
            {
                glassUValue = (glassUValues[0] as UValueGlassCentre).UValue;
                glassEdgeUValue = (glassEdgeUValues[0] as UValueGlassEdge).UValue;
            }

            List<FrameEdge> frameEdges = opening.Edges;
            List<double> frameAreas = new List<double>();
            List<double> frameUValues = new List<double>();
            List<double> edgeAreas = new List<double>();

            int h;
            int j;
            double we = 0.0635; //2.5" edge zone, per NFRC 100
            double totEdgeArea = 0;
            double totFrameArea = 0;

            for (int i = 0; i < frameEdges.Count; i++)
            {
                if (i == 0)
                {
                    h = frameEdges.Count-1;
                    j = i + 1;
                }
                else if (i == frameEdges.Count-1)
                {
                    h = i - 1;
                    j = 0;
                }
                else
                {
                    h = i - 1;
                    j = i + 1;
                } 
                double outerLength = frameEdges[i].Length();
                double wi = frameEdges[i].FrameEdgeProperty.WidthIntoOpening();
                double wh = frameEdges[h].FrameEdgeProperty.WidthIntoOpening();
                double wj = frameEdges[j].FrameEdgeProperty.WidthIntoOpening();
                double f_innerLength = outerLength - wj - wh;
                double f_area = wi * (outerLength + f_innerLength) / 2;
                double e_innerLength = f_innerLength - 2*we;
                double e_area = we * (f_innerLength + e_innerLength) / 2;
                frameAreas.Add(f_area);
                edgeAreas.Add(e_area);
                totEdgeArea += e_area;
                totFrameArea += f_area;

                List<IFragment> f_uValues = frameEdges[i].FrameEdgeProperty.GetAllFragments(typeof(UValueFrame));
                if (f_uValues.Count <= 0)
                {
                    BH.Engine.Base.Compute.RecordError($"Opening {opening.BHoM_Guid} does not have Frame U-value assigned.");
                    return null;
                }
                if (f_uValues.Count > 1)
                {
                    BH.Engine.Base.Compute.RecordError($"Opening {opening.BHoM_Guid} has more than one Frame U-value assigned.");
                    return null;
                }
                double frameUValue = (f_uValues[0] as UValueFrame).UValue;
                frameUValues.Add(frameUValue);
            }

            double totArea = opening.Area();
            if (totArea == 0)
            {
                BH.Engine.Base.Compute.RecordError($"Opening {opening.BHoM_Guid} has a calculated area of 0. Ensure the opening is valid with associated edges defining its geometry and try again.");
            }
            double glassArea = totArea - totEdgeArea - totFrameArea;
            double centerArea = totArea - totFrameArea;

            double edgeUValProduct = 0;
            double centerUValue = 0;

            if ((glassEdgeUValue > 0) && (glassUValue > 0))
            {
                for (int i = 0; i < edgeAreas.Count; i++)
                {
                    edgeUValProduct += (glassEdgeUValue * edgeAreas[i]);
                }
                centerUValue = (((glassArea * glassUValue) + edgeUValProduct) / centerArea);
                if (cavityUValue > 0)
                {
                    centerUValue = 1 / (1 / cavityUValue + 1 / centerUValue);
                }
            }
            else
            {
                centerUValue = cavityUValue;
            }
            double centerUValProduct = centerUValue * centerArea;

            double FrameUValProduct = 0;
            for (int i = 0; i < frameUValues.Count; i++)
            {
                FrameUValProduct += (frameUValues[i] * frameAreas[i]);
            }

            double baseUValue = ((centerUValProduct + FrameUValProduct) / totArea);

            double effectiveUValue = 0;
            if (contUValue == 0)
            {
                effectiveUValue = baseUValue;
            }
            else if (centerUValue == 0)
            {
                effectiveUValue = contUValue;
            }
            else
            {
                effectiveUValue = 1 / (1 / baseUValue + 1 / contUValue);
            }
            
            OverallUValue result = new OverallUValue (effectiveUValue, new List<IComparable> { opening.BHoM_Guid });
            return result;
        }

        /***************************************************/

    }
}


