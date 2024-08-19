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

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Structure;
using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.Engine.Geometry;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/


        [Description("Checks if a RetainignWall is valid by verifying that the Stem does not intersect the footing.")]
        [Input("retainingWall", "The RetainingWall to check.")]
        [Output("result", "Returns true if the RetainingWall is valid.")]
        public static bool IsValid(this RetainingWall retainingWall)
        {
            return IsValid(retainingWall.Stem, retainingWall.Footing);
        }

        /***************************************************/

        [Description("Checks if a Stem and PadFoundation are valid by verifying that the Stem does not intersect the footing.")]
        [Input("stem", "The Stem to check.")]
        [Input("footing", "The footing to check.")]
        [Output("result", "Returns true if the Stem and PadFoundation are valid.")]

        public static bool IsValid(this Stem stem, PadFoundation footing)
        {
            if (footing.IsNull() || stem.IsNull())
                return false;

            if (footing.TopOutline.ControlPoints().OrderBy(p => p.Z).First().Z - stem.Outline.ControlPoints().OrderBy(p => p.Z).First().Z > Tolerance.MicroDistance)
            {
                Base.Compute.RecordError("The footings highest control point is above the lowest control point of the stem. The two objects should not intersect.");
                return false;
            }

            return true;
        }

        /***************************************************/

        [Description("Checks if a Stem and PadFoundation are valid by performing null checks and a basic check that the stem does not go into the footing.")]
        [Input("retainingWall", "The RetainingWall to check.")]
        public static bool IsValid(this Stem stem, PadFoundation footing)
        {
            if (footing.IsNull() || stem.IsNull())
                return false;

            if (footing.TopOutline.ControlPoints().OrderBy(p => p.Z).First().Z - stem.Outline.ControlPoints().OrderBy(p => p.Z).First().Z > Tolerance.MicroDistance)
            {
                Base.Compute.RecordError("The footings highest control point is above the lowest control point of the stem. The two objects should not intersect.");
                return false;
            }

            return true;
        }
    }
}