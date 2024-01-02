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

        [Description("Returns effective U-Value of opening calculated using the Single Assessment Method (Using Psi-tj). Requires center of opening U-value and frame Psi-tj value as OpeningConstruction and FrameEdgeProperty fragments.")]
        [Input("opening", "Opening to find U-value for.")]
        [Output("effectiveUValue", "Effective U-value result of opening caclulated using SAM.")]
        public static OverallUValue UValueOpeningSAM(this Opening opening)
        {
            if (opening == null)
            {
                Base.Compute.RecordWarning("U Value can not be calculated for a null opening.");
                return null;
            }

            double area = opening.Area();

            List<IFragment> uValues = opening.OpeningConstruction.GetAllFragments(typeof(UValueGlassCentre));
            if (uValues.Count <= 0)
            {
                BH.Engine.Base.Compute.RecordError($"Opening {opening.BHoM_Guid} does not have U-value assigned.");
                return null;
            }
            if (uValues.Count > 1)
            {
                BH.Engine.Base.Compute.RecordError($"Opening {opening.BHoM_Guid} has more than one U-value assigned.");
                return null;
            }
            double uValue = (uValues[0] as UValueGlassCentre).UValue;

            List<FrameEdge> frameEdges = opening.Edges;
            List<double> psiValues = new List<double>();
            List<double> lengths = new List<double>();

            foreach (FrameEdge frameEdge in frameEdges)
            {
                List<IFragment> psiJoints = frameEdge.FrameEdgeProperty.GetAllFragments(typeof(PsiJoint));
                if (psiJoints.Count <= 0)
                {
                    BH.Engine.Base.Compute.RecordError($"One or more FrameEdges belonging to {opening.BHoM_Guid} does not have PsiJoint value assigned.");
                    return null;
                }
                if (psiJoints.Count > 1)
                {
                    BH.Engine.Base.Compute.RecordError($"One or more FrameEdges belonging to {opening.BHoM_Guid} has more than one PsiJoint value assigned. Each FrameEdge should only have one unique PsiJoint value assigned to it.");
                    return null;
                }
                double psiJoint = (psiJoints[0] as PsiJoint).PsiValue;
                psiValues.Add(psiJoint);

                double length = frameEdge.Length();
                lengths.Add(length);
            }

            double psiProduct = 0;
            for (int i = 0; i < lengths.Count; i++)
            {
                psiProduct = psiProduct + (lengths[i] * psiValues[i]);
            }

            double effectiveUValue = (((area * uValue) + psiProduct) / area);
            OverallUValue result = new OverallUValue(effectiveUValue, new List<IComparable> { opening.BHoM_Guid });
            return result;
        }

        /***************************************************/

    }
}


