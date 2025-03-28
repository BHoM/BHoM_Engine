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

using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using BH.oM.Quantities.Attributes;
using BH.oM.Structure.Elements;
using BH.oM.Structure.SectionProperties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Distributes a series of cellular openings along the centreline of the bar centreline. Method will fit in as many openings along the curve as it can, starting from the start of the curve.\n" +
                     "An empty list is returned if the bar does not contain a cellular section.")]
        [Input("bar", "Centreline curve to distribute the openings along.")]
        [Input("tolerance", "Tolerance used for checking how many openings that can be fitted along the centreline.", typeof(Length))]
        [Output("openingCurve", "The distributed cellular opening curves along the bar centreline.")]
        public static List<ICurve> CellularOpeningCurves(this Bar bar, double tolerance = Tolerance.Distance)
        {
            if (bar.IsNull())
                return null;

            CellularSection section = bar.SectionProperty as CellularSection;
            if (section == null)
                return new List<ICurve>();

            return Engine.Spatial.Query.DistributedOpeningCurves(section.Opening, bar.Centreline(), bar.Normal(), tolerance);
        }

        /***************************************************/
    }
}


