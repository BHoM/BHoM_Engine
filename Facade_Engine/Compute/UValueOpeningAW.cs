/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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
using BH.oM.Reflection;
using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Facade
{
    public static partial class Compute
    {
        /***************************************************/
        /****          Public Methods                   ****/
        /***************************************************/

        [Description("Returns effective U-Value of opening calculated using the Area Weighting Method. Requires center of opening U-value, frame U-value and edge U-value as Opening fragments.")]
        [Input("opening", "Opening to find U-value for.")]
        [Output("effectiveUValue", "Effective U-value result of opening calculated using area weighting.")]
        public static OverallUValue UValueOpeningAW(this Opening opening)
        {
            if (opening == null)
            {
                Reflection.Compute.RecordError($"U-Value can not be calculated for null opening.");
                return null;
            }

            List<IFragment> glassUValues = opening.GetAllFragments(typeof(UValueGlassCentre));

            if (glassUValues.Count <= 0)
            {
                BH.Engine.Reflection.Compute.RecordError($"Opening {opening.BHoM_Guid} does not have Glass U-value assigned.");
                return null;
            }
            if (glassUValues.Count > 1)
            {
                BH.Engine.Reflection.Compute.RecordError($"Opening {opening.BHoM_Guid} has more than one Glass U-value assigned.");
                return null;
            }
            double glassUValue = (glassUValues[0] as UValueGlassCentre).UValue;

            List<FrameEdge> frameEdges = opening.Edges;
            List<double> frameAreas = new List<double>();
            List<double> frameUValues = new List<double>();
            List<double> edgeAreas = new List<double>();
            List<double> edgeUValues = new List<double>();

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

                List<IFragment> f_uValues = frameEdges[i].GetAllFragments(typeof(UValueFrame));
                if (f_uValues.Count <= 0)
                {
                    BH.Engine.Reflection.Compute.RecordError($"Opening {opening.BHoM_Guid} does not have Frame U-value assigned.");
                    return null;
                }
                if (f_uValues.Count > 1)
                {
                    BH.Engine.Reflection.Compute.RecordError($"Opening {opening.BHoM_Guid} has more than one Frame U-value assigned.");
                    return null;
                }
                double frameUValue = (f_uValues[0] as UValueFrame).UValue;
                frameUValues.Add(frameUValue);

                List<IFragment> e_uValues = frameEdges[i].GetAllFragments(typeof(UValueGlassEdge));
                if (e_uValues.Count <= 0)
                {
                    BH.Engine.Reflection.Compute.RecordError($"Opening {opening.BHoM_Guid} does not have Frame U-value assigned.");
                    return null;
                }
                if (e_uValues.Count > 1)
                {
                    BH.Engine.Reflection.Compute.RecordError($"Opening {opening.BHoM_Guid} has more than one Frame U-value assigned.");
                    return null;
                }
                double edgeUValue = (e_uValues[0] as UValueGlassEdge).UValue;
                edgeUValues.Add(edgeUValue);
            }

            double glassArea = opening.Area() - totEdgeArea - totFrameArea;

            double FrameUValProduct = 0;
            double EdgeUValProduct = 0;
            for (int i = 0; i < frameUValues.Count; i++)
            {
                FrameUValProduct += (frameUValues[i] * frameAreas[i]);
                EdgeUValProduct += (edgeUValues[i] * edgeAreas[i]);
            }

            double totArea = opening.Area();
            if (totArea == 0)
            {
                BH.Engine.Reflection.Compute.RecordError($"Opening {opening.BHoM_Guid} has a calculated area of 0. Ensure the opening is valid with associated edges defining its geometry and try again.");
            }
            double effectiveUValue = (((glassArea * glassUValue) + EdgeUValProduct + FrameUValProduct) / totArea);
            OverallUValue result = new OverallUValue (effectiveUValue, new List<IComparable> { opening.BHoM_Guid });
            return result;
        }

        /***************************************************/

    }
}