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

        [Description("Returns effective U-Value of opening calculated using the Component Assessment Method (Using Psi-g). Requires center of opening U-value and frame Psi-tj value as OpeningConstruction and FrameEdgeProperty fragments.")]
        [Input("opening", "Opening to find U-value for.")]
        [Output("effectiveUValue", "Effective U-value result of opening calculated using CAM.")]
        public static OverallUValue UValueOpeningCAM(this Opening opening)
        {
            if (opening == null)
            {
                Base.Compute.RecordWarning("U Value can not be calculated for a null opening.");
                return null;
            }                
            
            double glassArea = opening.ComponentAreas().Item1;

            List<IFragment> glassUValues = opening.OpeningConstruction.GetAllFragments(typeof(UValueGlassCentre));
            if (glassUValues.Count <= 0)
            {
                BH.Engine.Base.Compute.RecordError($"Opening {opening.BHoM_Guid} does not have a Glass U-value assigned.");
                return null;
            }
            if (glassUValues.Count > 1)
            {
                BH.Engine.Base.Compute.RecordError($"Opening {opening.BHoM_Guid} has more than one Glass U-value assigned.");
                return null;
            }
            double glassUValue = (glassUValues[0] as UValueGlassCentre).UValue;

            List<FrameEdge> frameEdges = opening.Edges;
            List<double> frameAreas = new List<double>();
            List<double> frameUValues = new List<double>();
            List<double> psigLengths = new List<double>();
            List<double> psigValues = new List<double>();

            int h;
            int j;
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
                double wi = frameEdges[i].FrameEdgeProperty.Width();
                double wh = frameEdges[h].FrameEdgeProperty.Width();
                double wj = frameEdges[j].FrameEdgeProperty.Width();
                double innerLength = outerLength - wj - wh;
                double area = wi * (outerLength + innerLength) / 2;
                frameAreas.Add(area);
                psigLengths.Add(innerLength);

                List<IFragment> uValues = frameEdges[i].FrameEdgeProperty.GetAllFragments(typeof(UValueFrame));
                if (uValues.Count <= 0)
                {
                    BH.Engine.Base.Compute.RecordError($"Opening {opening.BHoM_Guid} does not have Frame U-value assigned.");
                    return null;
                }
                if (uValues.Count > 1)
                {
                    BH.Engine.Base.Compute.RecordError($"Opening {opening.BHoM_Guid} has more than one Frame U-value assigned.");
                    return null;
                }
                double frameUValue = (uValues[0] as UValueFrame).UValue;
                frameUValues.Add(frameUValue);
                
                List<IFragment> psiGs = frameEdges[i].FrameEdgeProperty.GetAllFragments(typeof(PsiGlassEdge));
                if (psiGs.Count <= 0)
                {
                    BH.Engine.Base.Compute.RecordError($"One or more FrameEdges belonging to {opening.BHoM_Guid} does not have PsiG value assigned.");
                    return null;
                }
                if (psiGs.Count > 1)
                {
                    BH.Engine.Base.Compute.RecordError($"One or more FrameEdges belonging to {opening.BHoM_Guid} has more than one PsiG value assigned. Each FrameEdge should only have one unique PsiG value assigned to it.");
                    return null;
                }
                double psiG = (psiGs[0] as PsiGlassEdge).PsiValue;
                psigValues.Add(psiG);
            }

            double psigProduct = 0;
            double FrameUValProduct = 0;
            for (int i = 0; i < psigLengths.Count; i++)
            {
                psigProduct += (psigLengths[i] * psigValues[i]);
                FrameUValProduct += (frameUValues[i] * frameAreas[i]);
            }

            double totArea = opening.Area();
            if (totArea == 0)
            {
                BH.Engine.Base.Compute.RecordError($"Opening {opening.BHoM_Guid} has a calculated area of 0. Ensure the opening is valid with associated edges defining its geometry and try again.");
            }
            double effectiveUValue = (((glassArea * glassUValue) + psigProduct + FrameUValProduct) / totArea);
            OverallUValue result = new OverallUValue (effectiveUValue, new List<IComparable> { opening.BHoM_Guid });
            return result;
        }

        /***************************************************/

    }
}


