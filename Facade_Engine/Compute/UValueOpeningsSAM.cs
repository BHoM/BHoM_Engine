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

        [Description("Returns effective U-Value of a collection of openings calculated using the Single Assessment Method (Using Psi-tj). Requires center of opening U-value and frame Psi-tj value as OpeningConstruction and FrameEdgeProperty fragments.")]
        [Input("openings", "Openings to find U-value for.")]
        [Output("effectiveUValue", "Effective total U-value result of opening calculated using SAM.")]
        public static OverallUValue UValueOpeningsSAM(this List<Opening> openings)
        {
            if (!openings.PsiValuesMatch())
            {
                BH.Engine.Base.Compute.RecordError("Openings have PsiValues at adjacent edges that do not match. SAM UValue method requires matching PsiValues at adjacent edges to work correctly.");
                return null;
            }

            double uValueProduct = 0;
            double totalArea = 0;
            foreach (Opening opening in openings)
            {
                double area = opening.Area();
                uValueProduct += opening.UValueOpeningSAM().UValue * area;
                totalArea += area;
            }
            if (totalArea == 0)
            {
                BH.Engine.Base.Compute.RecordError("Openings have a total calculated area of 0. Ensure Openings are valid with associated edges defining their geometry and try again.");
                return null;
            }

            double effectiveUValue =  uValueProduct / totalArea;
            OverallUValue result = new OverallUValue(effectiveUValue, openings.Select(x => x.BHoM_Guid as IComparable).ToList());
            return result;
        }

        /***************************************************/

    }
}


