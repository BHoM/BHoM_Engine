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
        [Description("Checks if a RetainignWall is valid by performing null checks and a basic check that the stem does not go into the footing.")]
        [Input("retainingWall", "The RetainingWall to check.")]
        [Output("result", "Returns true if the RetainingWall is valid.")]
        public static bool IsValid(this RetainingWall retainingWall)
        {
            if (retainingWall.IsNull())
                return false;
            if (retainingWall.Footing.IsNull() || retainingWall.Stem.IsNull())
                return false;

            //Checks if the footing is below the stem with a tolerence of 1E-6
            if (retainingWall.Footing.TopOutline.ControlPoints().OrderBy(p => p.Z).First().Z - 1E-6 < retainingWall.Stem.Outline.ControlPoints().OrderBy(p => p.Z).First().Z)
            {
                Base.Compute.RecordError("The footings highest control point is above the lowest control point of the stem. The two objects should not go into eachother. ");
                return false;
            }

            return true;
        }
    }
}