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

using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using BH.oM.Spatial.ShapeProfiles.CellularOpenings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Spatial
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the area of a single opening.")]
        [Input("opening", "The cellular opening to fetch the area from.")]
        [Output("area", "The area of a single opening.", typeof(Area))]
        public static double OpeningArea(this CircularOpening opening)
        {
            if (opening == null)
            {
                Base.Compute.RecordError("Unable to query the area from a null opening.");
                return double.NaN;
            }
            double r = opening.Diameter / 2;
            return r * r * Math.PI;
        }

        /***************************************************/

        [Description("Returns the area of a single opening.")]
        [Input("opening", "The cellular opening to fetch the area from.")]
        [Output("area", "The area of a single opening.", typeof(Area))]
        public static double OpeningArea(this HexagonalOpening opening)
        {
            if (opening == null)
            {
                Base.Compute.RecordError("Unable to query the area from a null opening.");
                return double.NaN;
            }
            return opening.Height * (opening.Width + opening.WidthWebPost) / 2 + opening.SpacerHeight * opening.Width;
        }

        /***************************************************/

        [Description("Returns the area of a single opening.")]
        [Input("opening", "The cellular opening to fetch the area from.")]
        [Output("area", "The area of a single opening.", typeof(Area))]
        public static double OpeningArea(this SinusoidalOpening opening)
        {
            if (opening == null)
            {
                Base.Compute.RecordError("Unable to query the area from a null opening.");
                return double.NaN;
            }
            double w = opening.SinusoidalLength * 2 + opening.WidthWebPost;
            return opening.Height * (opening.SinusoidalLength + opening.WidthWebPost);
        }

        /***************************************************/
        /**** Public Methods - Interface                ****/
        /***************************************************/

        [Description("Returns the area of a single opening.")]
        [Input("opening", "The cellular opening to fetch the area from.")]
        [Output("area", "The area of a single opening.", typeof(Area))]
        public static double IOpeningArea(this ICellularOpening opening)
        {
            if (opening == null)
            {
                Base.Compute.RecordError("Unable to query the area from a null opening.");
                return double.NaN;
            }
            return OpeningArea(opening as dynamic);
        }

        /***************************************************/
    }
}
