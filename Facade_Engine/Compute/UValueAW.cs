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
using BH.oM.Facade;
using BH.oM.Facade.Elements;
using BH.oM.Base;
using BH.Engine.Geometry;
using BH.Engine.Spatial;
using BH.Engine.Base;
using BH.oM.Facade.Fragments;

using BH.oM.Base.Attributes;
using BH.oM.Facade.Results;
using System.ComponentModel;

namespace BH.Engine.Facade
{
    public static partial class Compute
    {
        /***************************************************/
        /****          Public Methods                   ****/
        /***************************************************/
        
        [Description("Returns effective U-Value of a collection of Facade objects calculated using the Area Weighting Method. Requires center of opening U-value, frame U-value and edge U-value as OpeningConstruction and FrameEdgeProperty fragments for Openings, and UValueContinuous fragment for Panels.")]
        [Input("objs", "Objects to find U-value for.")]
        [Output("effectiveUValue", "Effective total U-value result of objects calculated using area weighting.")]
        public static OverallUValue UValueAW(this List<IFacadeObject> objs)
        {
            double uValueProduct = 0;
            double totalArea = 0;
            foreach (IFacadeObject obj in objs)
            {
                double area = 0;
                double uValue = 0;
                if (obj is Panel panel)
                {
                    area = panel.Area();
                    uValue = UValuePanelAW(panel).UValue;
                }
                else if (obj is Opening opening)
                {
                    area = opening.Area();
                    uValue = UValueOpeningAW(opening).UValue;
                }
                else
                {
                    Base.Compute.RecordWarning($"Object {obj.BHoM_Guid} is of a type currently not supported for UValue methods. It has been excluded from the calculation.");
                    continue;
                }
                uValueProduct += uValue * area;
                totalArea += area;
            }
            if (totalArea == 0)
            {
                Base.Compute.RecordError("Objects have a total calculated area of 0. Ensure Objects are valid with associated edges defining their geometry and try again.");
                return null;
            }

            double effectiveUValue = uValueProduct / totalArea;
            OverallUValue result = new OverallUValue(effectiveUValue, objs.Select(x => x.BHoM_Guid as IComparable).ToList());
            return result;
        }

        /***************************************************/

    }
}

