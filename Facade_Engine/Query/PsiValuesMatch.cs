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
using BH.oM.Geometry;
using BH.oM.Dimensional;
using BH.oM.Analytical.Elements;
using BH.Engine.Geometry;
using BH.oM.Base;
using System.Collections.Generic;
using BH.oM.Base.Attributes;
using System.ComponentModel;
using BH.oM.Facade.Elements;
using BH.oM.Facade.Fragments;
using BH.Engine.Base;

namespace BH.Engine.Facade
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns if all FrameEdges have matching Psi Values.")]
        [Input("openings", "Reference Openings.")]
        [Output("bool", "True if all adjacent edge PsiValues on the provided Openings match.")]
        public static bool PsiValuesMatch(this List<Opening> openings)
        {
            for (int i = 0; i < openings.Count; i++)
            {
                List<Opening> otherOpenings = openings;
                otherOpenings.RemoveAt(i);

                Opening o = openings[i];
                if(!o.PsiValuesMatch(otherOpenings))
                    return false;
            }

            return true;
        }

        /***************************************************/

        [Description("Returns if FrameEdges have matching Psi Values.")]
        [Input("opening", "Reference Opening.")]
        [Input("otherOpenings", "Other Openings to check PsiValue match for.")]
        [Output("bool", "True if all otherEdge's PsiValues match edge's PsiValue.")]
        public static bool PsiValuesMatch(this Opening opening, List<Opening> otherOpenings)
        {
            List<FrameEdge> edges = new List<FrameEdge>();

            foreach (Opening o in otherOpenings)
                edges.AddRange(o.Edges);
            
            foreach (FrameEdge edge in opening.Edges)
            {
                foreach (FrameEdge e in edges)
                {
                    if (edge.IsAdjacent(e) && !edge.PsiValuesMatch(e))
                        return false;
                }
            }

            return true;
        }

        /***************************************************/

        [Description("Returns if FrameEdges have matching Psi Values.")]
        [Input("edge", "Reference FrameEdge.")]
        [Input("otherEdges", "FrameEdges to check PsiValue for.")]
        [Output("bool", "True if all otherEdge's PsiValues match edge's PsiValue.")]
        public static bool PsiValuesMatch(this FrameEdge edge, List<FrameEdge> otherEdges)
        {
            if (edge == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query if PsiValues match if edge is null.");
                return false;
            }

            foreach (FrameEdge otherEdge in otherEdges)
            {
                if (!edge.PsiValuesMatch(otherEdge))
                    return false;
            }

            return true;
        }

        /***************************************************/

        [Description("Returns if FrameEdges have matching Psi Values.")]
        [Input("edge", "Reference FrameEdge.")]
        [Input("otherEdge", "FrameEdge to check PsiValue for.")]
        [Output("bool", "True if otherEdge's PsiValue matches edge's PsiValue.")]
        public static bool PsiValuesMatch (this FrameEdge edge, FrameEdge otherEdge)
        {
            if(edge == null || otherEdge == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query if PsiValues match if either edge is null.");
                return false;
            }

            List<IFragment> psiRef = edge.FrameEdgeProperty.GetAllFragments(typeof(PsiJoint));
            List<IFragment> psiOther = otherEdge.FrameEdgeProperty.GetAllFragments(typeof(PsiJoint));

            if (psiRef.Count == 0)
            {
                BH.Engine.Base.Compute.RecordError($"FrameEdge with guid {edge.BHoM_Guid} has no Psi Value assigned. Method requires a PsiJoint fragment assigned to the element.");
                return false;
            }
            if (psiOther.Count == 0)
            {
                BH.Engine.Base.Compute.RecordError($"FrameEdge with guid {otherEdge.BHoM_Guid} has no Psi Value assigned. Method requires a PsiJoint fragment assigned to the element.");
                return false;
            }

            if (psiRef.Count > 1)
            {
                BH.Engine.Base.Compute.RecordError($"FrameEdge with guid {edge.BHoM_Guid} has multiple Psi Value assigned. Method requires a signle PsiJoint fragment assigned to the element.");
                return false;
            }
            if (psiOther.Count >1)
            {
                BH.Engine.Base.Compute.RecordError($"FrameEdge with guid {edge.BHoM_Guid} has multiple Psi Value assigned. Method requires a signle PsiJoint fragment assigned to the element.");
                return false;
            }

            PsiJoint psiVal = (PsiJoint)psiRef[0];
            PsiJoint psiValOther = (PsiJoint)psiOther[0];

            return psiVal.PsiValue == psiValOther.PsiValue;
        }

        /***************************************************/
    }
}


